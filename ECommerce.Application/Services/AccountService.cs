using AutoMapper;
using ECommerce.Core.Common;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Dtos.AuthDtos;
using ECommerce.Core.Dtos.ProfileDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.OrderModule.Owned;
using ECommerce.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System.Net;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class AccountService(
    UserManager<AppUser> userManager,
    IEmailSender emailSender,
    IStringLocalizer<AccountService> localizer,
    IConfiguration config,
    IMapper mapper,
    IImageUploader imageUploader,
    ICacheService cacheService)
    : IAccountService
{
    public async Task<AccountInfoResult> GetAccountInfoAsync(string userId)
    {
        var user = await userManager.FindByIdWithIncludesAsync(userId, u => u.Address);
        return mapper.Map<AccountInfoResult>(user);
    }

    public async Task<ShippingAddress> GetUserShippingAddressAsync(string userId)
    {
        var user = await userManager.FindByIdWithIncludesAsync(userId, u => u.Address);
        return new ShippingAddress()
        {
            RecipientName = user?.DisplayName ?? string.Empty,
            PhoneNumber = user?.PhoneNumber ?? string.Empty,
            Street = user?.Address?.Street ?? string.Empty,
            City = user?.Address?.City ?? string.Empty,
            Country = user?.Address?.Country ?? string.Empty,
        };
    }

    public async Task<string> ForgetPasswordAsync(ForgetPasswordInput forgetPassword)
    {
        var user = await userManager.FindByEmailAsync(forgetPassword.Email);
        if (user is not null)
        {
            var userToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebUtility.UrlEncode(userToken);

            var resetUrl =
                $"{config["ClientUrl"]}/auth/reset-password?email={forgetPassword.Email}&token={encodedToken}";

            var email = GeneratePasswordResetEmail(forgetPassword.Email, resetUrl, true);

            await emailSender.SendEmailAsync(email);
        }

        return localizer[L.Account.ResetPassword.EmailSent];
    }

    public async Task<PasswordResetResult> ResetPasswordAsync(PasswordResetInput resetPassword)
    {
        var user = await userManager.FindByEmailAsync(resetPassword.Email);
        if (user is null)
            return new PasswordResetResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = localizer[L.Account.ResetPassword.InvalidResetLink]
            };

        var result = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
        if (!result.Succeeded)
            return new PasswordResetResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = localizer[L.Account.ResetPassword.InvalidResetLink]
            };

        return new PasswordResetResult()
        {
            Status = StatusCodes.Status200OK,
            Message = localizer[L.Account.ResetPassword.PasswordResetSuccess]
        };
    }

    public async Task<ProfileUpdateResult> ChangePasswordAsync(PasswordChangeInput input, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return new ProfileUpdateResult { Message = localizer[L.Account.UserNotFound] };

        var result = await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new ProfileUpdateResult { Message = localizer[L.Account.PasswordChange.ChangeFailed, errors] };
        }

        return new ProfileUpdateResult
        {
            Updated = true,
            Message = localizer[L.Account.PasswordChange.ChangeSuccess]
        };
    }

    public async Task<ProfileUpdateResult> UpdateBasicInfoAsync(BasicInfoUpdateInput input, string userId)
    {
        var user = await userManager.FindByIdWithIncludesAsync(userId, u => u.Address);
        if (user is null) return new ProfileUpdateResult { Message = localizer[L.Account.UserNotFound] };

        var (isChanged, errorMessage) = await UpdateBasicsAsync(input, user);
        if (!string.IsNullOrEmpty(errorMessage)) return new ProfileUpdateResult { Message = errorMessage };

        if (!isChanged) return new ProfileUpdateResult { Message = localizer[L.Account.BasicInfo.NoChanges] };

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new ProfileUpdateResult { Message = localizer[L.Account.BasicInfo.UpdateFailed] };
        }

        return new ProfileUpdateResult
        {
            Updated = true,
            Message = localizer[L.Account.BasicInfo.UpdateSuccess]
        };
    }

    public async Task<(int Status, string Message)> RequestEmailChangeAsync(string userId, EmailChangeInput input)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return (StatusCodes.Status404NotFound, localizer[L.Account.UserNotFound]);

        if (input.NewEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            return (StatusCodes.Status400BadRequest, localizer[L.Account.EmailChange.SameEmail]);

        if (!await userManager.CheckPasswordAsync(user, input.Password))
            return (StatusCodes.Status400BadRequest, localizer[L.Account.EmailChange.IncorrectPassword]);

        var existingUser = await userManager.FindByEmailAsync(input.NewEmail);
        if (existingUser is not null)
            return (StatusCodes.Status409Conflict, localizer[L.Account.EmailChange.EmailTaken]);

        var verificationCode = GenerateVerificationCode();

        var cacheKey = $"{userId}_{verificationCode}";
        var cacheVal = new EmailChangeCache()
        {
            UserId = userId,
            NewEmail = input.NewEmail,
            VerificationCode = verificationCode,
        };

        await cacheService.SetCacheAsync(cacheKey, cacheVal, TimeSpan.FromMinutes(15));

        var _verificationCodeEmail = GenerateVerificationCodeEmail(
            input.NewEmail,
            user.Email ?? string.Empty,
            verificationCode,
            isHtml: true);

        await emailSender.SendEmailAsync(_verificationCodeEmail);

        return (StatusCodes.Status200OK, localizer[L.Account.EmailChange.VerificationSent]);
    }

    public async Task<(int Status, string Message)> ConfirmEmailChangeAsync(string currentUserId,
        string verificationCode)
    {
        var user = await userManager.FindByIdAsync(currentUserId);
        if (user is null)
            return (StatusCodes.Status404NotFound, localizer[L.Account.UserNotFound]);

        var cachedKey = $"{user.Id}_{verificationCode}";
        var cachedData = await cacheService.GetCacheAsync<EmailChangeCache>(cachedKey);
        if (cachedData is null)
            return (StatusCodes.Status400BadRequest, localizer[L.Account.EmailChange.InvalidOrExpiredCode]);

        var token = await userManager.GenerateChangeEmailTokenAsync(user, cachedData.NewEmail);

        var result = await userManager.ChangeEmailAsync(user, cachedData.NewEmail, token);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return (StatusCodes.Status400BadRequest, localizer[L.Account.EmailChange.ChangeFailed, errors]);
        }

        await cacheService.RemoveCacheAsync(cachedKey);
        return (StatusCodes.Status200OK, localizer[L.Account.EmailChange.ChangeSuccess]);
    }

    #region Helpers

    private async Task<(bool IsChanged, string? ErrorMessage)> UpdateBasicsAsync(BasicInfoUpdateInput input,
        AppUser user)
    {
        bool isChanged = false;
        string? errorMessage = null;

        if (input.Avatar is not null)
        {
            var uploadResult = await imageUploader.UploadImageAsync(input.Avatar, "images/users");
            if (!uploadResult.Uploaded)
            {
                errorMessage = uploadResult.ErrorMessage;
                return (isChanged, errorMessage);
            }

            if (!string.IsNullOrEmpty(user.PictureUrl))
                imageUploader.DeleteFile(user.PictureUrl);

            user.PictureUrl = uploadResult.FilePath;
            isChanged = true;
        }


        if (input.DisplayName is not null && user.DisplayName != input.DisplayName)
        {
            user.DisplayName = input.DisplayName;
            isChanged = true;
        }

        if (input.PhoneNumber is not null && user.PhoneNumber != input.PhoneNumber)
        {
            user.PhoneNumber = input.PhoneNumber;
            isChanged = true;
        }

        if (input.Address is not null)
        {
            user.Address ??= new Address();

            if (user.Address.Street != input.Address.Street)
            {
                user.Address.Street = input.Address.Street;
                isChanged = true;
            }

            if (user.Address.City != input.Address.City)
            {
                user.Address.City = input.Address.City;
                isChanged = true;
            }

            if (user.Address.Country != input.Address.Country)
            {
                user.Address.Country = input.Address.Country;
                isChanged = true;
            }
        }

        return (isChanged, errorMessage);
    }

    private static Email GeneratePasswordResetEmail(string recipientEmail, string resetUrl, bool isHtml = false)
    {
        const string subject = "Password Reset Request";

        var htmlBody = $@"
			<div style='font-family:Segoe UI, Roboto, sans-serif; max-width:600px; margin:auto; padding:20px; border:1px solid #e0e0e0; border-radius:8px; background-color:#ffffff;'>
				<h2 style='color:#2c3e50;'>{subject}</h2>
				<p style='font-size:15px; color:#333;'>You requested a password reset. Click the button below to reset your password:</p>
				<p style='text-align:center; margin:30px 0;'>
					<a href='{resetUrl}' style='background-color:#007bff; color:#ffffff; padding:12px 24px; text-decoration:none; border-radius:5px; font-weight:bold; display:inline-block;'>
						Reset Password
					</a>
				</p>
				<p style='font-size:14px; color:#555;'>This link is valid for 1 usage.</p>
				<hr style='margin:20px 0; border:0; border-top:1px solid #e0e0e0;' />
				<p style='font-size:13px; color:#999;'>If you didn't request this, you can safely ignore this email.</p>
			</div>";

        var textBody = $@"
			{subject}
			Password Reset Request
			You requested a password reset. Click the link below to reset your password:
			{resetUrl}
			This link is valid for 1 usage.
			If you didn't request this, you can safely ignore this email.";

        return new Email()
        {
            Subject = "Password Reset Request",
            To = [recipientEmail],
            IsHtml = isHtml,
            Body = isHtml ? htmlBody : textBody
        };
    }

    private static string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(1_000, 10_0000).ToString();
    }

    private static Email GenerateVerificationCodeEmail(string recipientEmail, string currentEmail, string code,
        bool isHtml = false)
    {
        const string subject = "Email Change Verification";


        var htmlBody = $@"
			<div style='font-family:Segoe UI, Roboto, sans-serif; max-width:600px; margin:auto; padding:20px; border:1px solid #e0e0e0; border-radius:8px; background-color:#ffffff;'>
				<h2 style='color:#2c3e50;'>{subject}</h2>
				<p style='font-size:15px; color:#333;'>Hello,</p>
				<p style='font-size:15px; color:#333;'>You have requested to change your email address from 
					<strong>{currentEmail}</strong> to this email address.</p>
				<p style='text-align:center; margin:30px 0;'>
					<span style='display:inline-block; font-size:24px; font-weight:bold; color:#007bff; padding:12px 24px; border:1px dashed #007bff; border-radius:5px;'>
						{code}
					</span>
				</p>
				<p style='font-size:14px; color:#555;'>This code will expire in 15 minutes.</p>
				<hr style='margin:20px 0; border:0; border-top:1px solid #e0e0e0;' />
				<p style='font-size:13px; color:#999;'>If you didn't request this change, you can safely ignore this email.</p>
				<p style='font-size:13px; color:#999;'>Best regards,<br>Fresh Cart Team</p>
			</div>";

        var textBody = $@"
			{subject}
			Hello,

			You have requested to change your email address from {currentEmail} to this email.

			Your verification code is: {code}

			This code will expire in 15 minutes.

			If you didn't request this change, you can safely ignore this email.

			Best regards,
			Fresh Cart Team";

        return new Email
        {
            Subject = subject,
            To = [recipientEmail],
            IsHtml = isHtml,
            Body = isHtml ? htmlBody : textBody
        };
    }

    #endregion
}
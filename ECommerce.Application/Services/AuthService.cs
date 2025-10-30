using AutoMapper;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Dtos.AuthDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Infrastructure.Database;
using ECommerce.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ECommerce.Application.Services;

public class AuthService(
    ITokenService tokenService,
    UserManager<AppUser> userManager,
    AppDbContext appDbContext,
    IConfiguration config,
    IEmailSender emailSender,
    IImageUploader imageUploader,
    IMapper mapper,
    ICacheService cacheService,
    IStringLocalizer<AuthService> localizer)
    : IAuthService
{
    private readonly IConfiguration _config = config;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IImageUploader _imageUploader = imageUploader;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;


    public async Task<AuthResult> RegisterUserAsync(RegisterInput register)
    {
        if (await userManager.FindByEmailAsync(register.Email) is not null)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status409Conflict,
                Message = localizer[L.Auth.Registration.EmailAlreadyExists]
            };
        }

        if (await userManager.FindByNameAsync(register.UserName) is not null)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status409Conflict,
                Message = localizer[L.Auth.Registration.UserNameAlreadyExists]
            };
        }

        var user = new AppUser()
        {
            DisplayName = register.DisplayName,
            UserName = register.UserName,
            Email = register.Email,
            PhoneNumber = register.PhoneNumber,
            Address = new Address()
            {
                Street = register.Address.Street,
                City = register.Address.City,
                Country = register.Address.Country,
            }
        };


        var creationResult = await userManager.CreateAsync(user, register.Password);
        if (!creationResult.Succeeded)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = localizer[L.Auth.Registration.FailedToCreateUser, user.DisplayName],
                Errors = [.. creationResult.Errors.Select(e => e.Description)]
            };
        }

        var addedToRoleResult = await userManager.AddToRoleAsync(user, RolesConstants.User);
        if (!addedToRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            return new AuthResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Message =
                    localizer[L.Auth.Registration.FailedToAssignUserToRole, user.DisplayName, RolesConstants.User],
                Errors = [.. addedToRoleResult.Errors.Select(e => e.Description)]
            };
        }

        var refreshToken = tokenService.GenerateRefreshToken();

        return new AuthResult()
        {
            Status = StatusCodes.Status201Created,
            Message = localizer[L.Auth.Registration.UserCreatedSuccessfully, user.DisplayName],
            Token = await tokenService.GenerateJwtTokenAsync(user),
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresOn = refreshToken.ExpiresOn,
            Roles = [.. await userManager.GetRolesAsync(user)]
        };
    }

    public async Task<AuthResult> LoginUserAsync(LoginInput login)
    {
        var user = login.UserNameOrEmail.Contains('@')
            ? await userManager.FindByEmailWithIncludesAsync(login.UserNameOrEmail, u => u.RefreshTokens)
            : await userManager.FindByNameWithIncludesAsync(login.UserNameOrEmail, u => u.RefreshTokens);

        if (user is null || !await userManager.CheckPasswordAsync(user, login.Password))
            return new AuthResult()
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = localizer[L.Auth.Login.InvalidLogin]
            };

        var authDto = new AuthResult()
        {
            Status = StatusCodes.Status200OK,
            Message = localizer[L.Auth.Login.WelcomeUser, user.DisplayName],
            Token = await tokenService.GenerateJwtTokenAsync(user),
            Roles = [.. await userManager.GetRolesAsync(user)]
        };

        var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
        if (activeRefreshToken is not null)
        {
            authDto.RefreshToken = activeRefreshToken.Token;
            authDto.RefreshTokenExpiresOn = activeRefreshToken.ExpiresOn;
            return authDto;
        }

        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(newRefreshToken);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = localizer[L.Auth.Token.RefreshTokenFailed]
            };
        }

        authDto.RefreshToken = newRefreshToken.Token;
        authDto.RefreshTokenExpiresOn = newRefreshToken.ExpiresOn;
        return authDto;
    }

    public async Task<AuthResult> RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = localizer[L.Auth.Token.RefreshTokenRequired]
            };
        }

        var user = await userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user is null)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = localizer[L.Auth.Token.RefreshTokenRequired]
            };
        }


        var userRefreshToken = user.RefreshTokens.First(t => t.Token == refreshToken);

        if (!userRefreshToken.IsActive)
        {
            return new AuthResult()
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = localizer[L.Auth.Token.InactiveToken]
            };
        }

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = tokenService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);

        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
            return new AuthResult()
            {
                Status = StatusCodes.Status200OK,
                Token = await tokenService.GenerateJwtTokenAsync(user),
                Message = localizer[L.Auth.Token.TokenRefreshed],
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresOn = newRefreshToken.ExpiresOn,
                Roles = [.. await userManager.GetRolesAsync(user)]
            };

        userRefreshToken.RevokedOn = null;
        user.RefreshTokens.Remove(newRefreshToken);
        await userManager.UpdateAsync(user);

        return new AuthResult()
        {
            Status = StatusCodes.Status401Unauthorized,
            Message = localizer[L.Auth.Token.RefreshTokenFailed]
        };
    }

    public async Task<bool> RevokeTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return false;

        var existedToken = await appDbContext
            .Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (existedToken is null) return false;

        if (existedToken.IsRevoked) return true;

        existedToken.RevokedOn = DateTime.UtcNow;

        var rowsAffected = await appDbContext.SaveChangesAsync();

        return rowsAffected > 0;
    }
}
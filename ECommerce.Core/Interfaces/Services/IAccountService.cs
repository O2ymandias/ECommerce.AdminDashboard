using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.AuthDtos;
using ECommerce.Core.Dtos.ProfileDtos;
using ECommerce.Core.Models.OrderModule.Owned;

namespace ECommerce.Core.Interfaces.Services;

public interface IAccountService
{
    Task<AccountInfoResult> GetAccountInfoAsync(string userId);
    Task<ShippingAddress> GetUserShippingAddressAsync(string userId);
    Task<string> ForgetPasswordAsync(ForgetPasswordInput forgetPassword);
    Task<PasswordResetResult> ResetPasswordAsync(PasswordResetInput resetPassword);
    Task<ProfileUpdateResult> ChangePasswordAsync(PasswordChangeInput input, string userId);
    Task<ProfileUpdateResult> UpdateBasicInfoAsync(BasicInfoUpdateInput input, string userId);
    Task<(int Status, string Message)> RequestEmailChangeAsync(string userId, EmailChangeInput input);
    Task<(int Status, string Message)> ConfirmEmailChangeAsync(string currentUserId, string verificationCode);
}
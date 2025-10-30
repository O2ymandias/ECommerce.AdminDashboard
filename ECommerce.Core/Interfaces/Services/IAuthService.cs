using ECommerce.Core.Dtos.AuthDtos;

namespace ECommerce.Core.Interfaces.Services;
public interface IAuthService
{
	Task<AuthResult> RegisterUserAsync(RegisterInput register);
	Task<AuthResult> LoginUserAsync(LoginInput login);
	Task<AuthResult> RefreshTokenAsync(string? refreshToken);
	Task<bool> RevokeTokenAsync(string? refreshToken);
}

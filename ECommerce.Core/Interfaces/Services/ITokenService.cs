using ECommerce.Core.Models.AuthModule;

namespace ECommerce.Core.Interfaces.Services;
public interface ITokenService
{
	Task<string> GenerateJwtTokenAsync(AppUser appUser);
	RefreshToken GenerateRefreshToken();
}

using ECommerce.Core.Common.Options;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Application.Services;

public class TokenService(
    UserManager<AppUser> userManager,
    IOptions<JwtOptions> jwtOptions,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
    : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly RefreshTokenOptions _refreshTokenOptions = refreshTokenOptions.Value;

    public async Task<string> GenerateJwtTokenAsync(AppUser appUser)
    {
        // Registered Claims
        var registeredClaims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Sub, appUser.Id),
            new(JwtRegisteredClaimNames.UniqueName, appUser.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Name, appUser.DisplayName),
            new(JwtRegisteredClaimNames.Email, appUser.Email ?? string.Empty),
        };

        // Private Claims
        var userClaims = await userManager.GetClaimsAsync(appUser);
        var userRoles = await userManager.GetRolesAsync(appUser);
        var privateClaims = new List<Claim>(userClaims);
        foreach (var role in userRoles)
            privateClaims.Add(new Claim("role", role));

        var allClaims = registeredClaims.Concat(privateClaims);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: allClaims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.MinutesToExpire),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[_refreshTokenOptions.Length];
        rng.GetBytes(bytes);
        return new RefreshToken()
        {
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenOptions.DaysToExpire),
            Token = Convert.ToBase64String(bytes)
        };
    }
}
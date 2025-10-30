using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Dtos.AuthDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResult>> Register(RegisterInput register)
    {
        var result = await authService.RegisterUserAsync(register);

        if (result.Status == StatusCodes.Status201Created)
        {
            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
            return StatusCode(result.Status, result);
        }

        return result.Status switch
        {
            StatusCodes.Status400BadRequest => BadRequest(errorFactory.CreateValidationErrorResponse(result.Errors)),
            StatusCodes.Status409Conflict => Conflict(errorFactory.CreateErrorResponse(result.Status, result.Message)),
            _ => StatusCode(result.Status)
        };
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResult>> Login(LoginInput loginDto)
    {
        var result = await authService.LoginUserAsync(loginDto);

        if (result.Status != StatusCodes.Status200OK)
            return Unauthorized(errorFactory.CreateErrorResponse(result.Status, result.Message));

        SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
        return Ok(result);
    }


    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResult>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var result = await authService.RefreshTokenAsync(refreshToken);

        if (result.Status != StatusCodes.Status200OK)
            return result.Status switch
            {
                StatusCodes.Status400BadRequest =>
                    BadRequest(errorFactory.CreateErrorResponse(result.Status, result.Message)),

                StatusCodes.Status401Unauthorized =>
                    Unauthorized(errorFactory.CreateErrorResponse(result.Status, result.Message)),

                _ => StatusCode(result.Status)
            };

        SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiresOn);
        return Ok(result);
    }

    [HttpGet("revoke-token")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> RevokeToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var result = await authService.RevokeTokenAsync(refreshToken);
        return Ok(result);
    }

    private void SetRefreshTokenInCookies(string refreshToken, DateTime expiresOn)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Expires = expiresOn.ToLocalTime(),
        });
    }
}
using ECommerce.Core.Common.Constants;
using Microsoft.Extensions.Localization;

namespace ECommerce.APIs.ResponseModels.ErrorModels;

public class ApiErrorResponseFactory(IStringLocalizer<ApiErrorResponse> localizer, IWebHostEnvironment env)
    : IApiErrorResponseFactory
{
    public ApiErrorResponse CreateErrorResponse(int statusCode, string? message = null)
    {
        var defaultMessage = GenerateDefaultStatusMessage(statusCode);
        return new ApiErrorResponse(statusCode, message ?? defaultMessage);
    }

    public ApiExceptionErrorResponse CreateExceptionErrorResponse(Exception ex)
    {
        var defaultMessage = GenerateDefaultStatusMessage(StatusCodes.Status500InternalServerError);
        return new ApiExceptionErrorResponse(ex, env, defaultMessage);
    }

    public ApiValidationErrorResponse CreateValidationErrorResponse(IEnumerable<string> errors)
    {
        var defaultMessage = GenerateDefaultStatusMessage(StatusCodes.Status400BadRequest);
        return new ApiValidationErrorResponse(errors, defaultMessage);
    }

    private string GenerateDefaultStatusMessage(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest =>
                localizer[L.ApiErrors.BadRequest] ?? "The request was invalid or cannot be served.",

            StatusCodes.Status401Unauthorized =>
                localizer[L.ApiErrors.Unauthorized] ??
                "Authentication is required and has failed or has not yet been provided.",

            StatusCodes.Status403Forbidden =>
                localizer[L.ApiErrors.Forbidden] ?? "You do not have permission to access this resource.",

            StatusCodes.Status404NotFound =>
                localizer[L.ApiErrors.NotFound] ?? "The requested resource could not be found.",

            StatusCodes.Status500InternalServerError =>
                localizer[L.ApiErrors.InternalServerError] ?? "An unexpected error occurred on the server.",

            _ => localizer[L.ApiErrors.GenericError] ?? "An error occurred.",
        };
    }
}
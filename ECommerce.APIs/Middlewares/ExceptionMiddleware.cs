using ECommerce.APIs.ResponseModels.ErrorModels;

namespace ECommerce.APIs.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IApiErrorResponseFactory errorFactory)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error: {Message} occurred at {Endpoint}",
                ex.Message,
                context.GetEndpoint()?.DisplayName ?? string.Empty
            );

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var error = errorFactory.CreateExceptionErrorResponse(ex);
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
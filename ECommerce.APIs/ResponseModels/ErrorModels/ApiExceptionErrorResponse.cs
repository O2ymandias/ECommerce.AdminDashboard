namespace ECommerce.APIs.ResponseModels.ErrorModels;

public class ApiExceptionErrorResponse : ApiErrorResponse
{
    public ApiExceptionErrorResponse(Exception ex, IWebHostEnvironment env, string message)
        : base(StatusCodes.Status500InternalServerError, message)
    {
        if (env.IsDevelopment())
        {
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }
    }

    public string? StackTrace { get; }
}
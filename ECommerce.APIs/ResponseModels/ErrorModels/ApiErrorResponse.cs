namespace ECommerce.APIs.ResponseModels.ErrorModels;

public class ApiErrorResponse(int statusCode, string message)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
}
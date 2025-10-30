namespace ECommerce.APIs.ResponseModels.ErrorModels;

public class ApiValidationErrorResponse(IEnumerable<string> errors, string message)
    : ApiErrorResponse(StatusCodes.Status400BadRequest, message)
{
    public List<string> Errors { get; set; } = [.. errors];
}
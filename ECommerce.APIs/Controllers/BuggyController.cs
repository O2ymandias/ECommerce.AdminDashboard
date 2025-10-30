using ECommerce.APIs.ResponseModels.ErrorModels;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BuggyController(IConnectionMultiplexer connection, IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    private readonly IDatabase _redis = connection.GetDatabase();

    [HttpGet("notfound")]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult NotFoundResponse()
    {
        var error = errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound);
        return NotFound(error);
    }

    [HttpGet("servererror")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ServerErrorResponse()
    {
        throw new Exception("Internal Server Error");
    }

    [HttpGet("badrequest")]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public IActionResult BadRequestResponse()
    {
        var error = errorFactory.CreateValidationErrorResponse(["Age exceeds 30 years old"]);
        return BadRequest(error);
    }

    [HttpGet("badrequest/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public IActionResult BadRequestResponse(int id) => Ok();

    [HttpGet("unauthorized")]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public IActionResult UnauthorizedResponse()
    {
        var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
        return Unauthorized(error);
    }

    [HttpGet("redis")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TestRedisConnection()
    {
        var message = await _redis.StringGetAsync(new RedisKey("message"));
        var error = errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound);
        if (message.IsNull) return NotFound(error);
        return Ok(message.ToString());
    }
}
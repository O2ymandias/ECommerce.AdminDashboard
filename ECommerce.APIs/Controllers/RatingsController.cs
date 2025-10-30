using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.APIs.ResponseModels.ErrorModels;

namespace ECommerce.APIs.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RatingsController(IRatingService ratingService, IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(RatingPaginationResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<RatingPaginationResult>> GetRatings([FromQuery] RatingSpecsParams specsParams) =>
        Ok(await ratingService.GetRatingsAsync(specsParams));

    [AllowAnonymous]
    [HttpGet("average/{productId}")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    public async Task<ActionResult<double>> GetAverageRatingAsync([FromRoute] int productId) =>
        Ok(await ratingService.GetAverageRatingAsync(productId));


    [HttpGet("latest")]
    [ProducesResponseType(typeof(LatestRatingsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LatestRatingsResult>> GetLatestRatings(int limit)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await ratingService.GetLatestRatingsAsync(userId, limit));
    }

    [HttpPost]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> AddRating(RatingInput ratingInput)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await ratingService.AddRatingAsync(ratingInput, userId));
    }

    [HttpDelete("{ratingId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> DeleteRating(int ratingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await ratingService.DeleteRatingAsync(ratingId));
    }
}
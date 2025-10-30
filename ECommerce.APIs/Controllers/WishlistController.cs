using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.WishlistDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.APIs.ResponseModels.ErrorModels;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController(IWishlistService wishlistService, IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<WishlistItemResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginationResult<WishlistItemResult>>> GetUserWishListItems(
        [FromQuery] WishlistItemSpecsParams specsParams
    )
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        specsParams.UserId = userId;

        return Ok(await wishlistService.GetUserWishListItemsAsync(specsParams));
    }

    [HttpGet("product-ids")]
    [ProducesResponseType(typeof(IReadOnlyList<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<int>>> GetWishListProductIds()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await wishlistService.GetWishlistProductIdsAsync(userId));
    }

    [HttpGet("total")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> GetTotal()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await wishlistService.GetTotalAsync(userId));
    }

    [HttpPost("add-to-wishlist")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> AddToWishlist([FromBody] WishlistItemInput input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await wishlistService.AddToWishListAsync(input, userId));
    }

    [HttpDelete("remove-from-wishlist")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> RemoveFromWishlist([FromBody] WishlistItemInput input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await wishlistService.DeleteFromWishlistAsync(input, userId));
    }

    [HttpDelete("clear-wishlist")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> ClearWishlist()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        return Ok(await wishlistService.ClearWishlistAsync(userId));
    }
}
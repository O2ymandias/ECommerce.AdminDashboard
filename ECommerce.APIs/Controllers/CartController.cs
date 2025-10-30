using ECommerce.APIs.ResponseModels;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Dtos.CartDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(ICartService cartService, IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResult>> GetCart(string id)
    {
        var result = await cartService.GetCartAsync(id);
        return result is not null
            ? Ok(result)
            : NotFound(errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound));
    }

    [HttpPost("add-to-cart")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiMessageResponse>> AddToCart([FromBody] CartItemInput item)
    {
        var result = await cartService.AddToCartAsync(item);
        return result.Updated
            ? Ok(new ApiMessageResponse() { Message = result.Message })
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }

    [HttpDelete("remove-from-cart")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiMessageResponse>> RemoveFromCart([FromBody] CartItemInput item)
    {
        var result = await cartService.RemoveFromCartAsync(item);
        return result.Updated
            ? Ok(new { result.Message })
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }

    [HttpPut("update-quantity")]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiMessageResponse>> UpdateQuantity([FromBody] UpdateQuantityInput item)
    {
        var result = await cartService.UpdateQuantityAsync(item);
        return result.Updated
            ? Ok(new { result.Message })
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeleteCart(string id) => Ok(await cartService.DeleteCartAsync(id));
}
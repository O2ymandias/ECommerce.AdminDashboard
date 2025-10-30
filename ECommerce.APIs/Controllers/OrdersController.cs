using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;

namespace ECommerce.APIs.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(
    IOrderService orderService,
    ICheckoutService checkoutService,
    IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [Authorize(Roles = RolesConstants.User)]
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateOrderResult>> CreateOrder([FromBody] CreateOrderInput input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        var result = await orderService.CreateOrderAsync(input, userId);

        return result.Success
            ? Ok(result)
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }

    [Authorize(Roles = $"{RolesConstants.User},{RolesConstants.Admin}")]
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<OrderResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginationResult<OrderResult>>> GetOrders([FromQuery] OrderSpecsParams specsParams)
    {
        if (User.IsInRole(RolesConstants.Admin))
            return Ok(await orderService.GetOrdersAsync(specsParams));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        specsParams.UserId = userId;
        return await orderService.GetOrdersAsync(specsParams);
    }

    [Authorize(Roles = $"{RolesConstants.User},{RolesConstants.Admin}")]
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResult>> GetOrder(int orderId, string? userId)
    {
        if (!User.IsInRole(RolesConstants.Admin))
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
                return Unauthorized(error);
            }
        }

        var result = await orderService.GetOrderAsync(orderId, userId);

        return result is not null
            ? Ok(result)
            : NotFound(errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound));
    }

    [Authorize(Roles = $"{RolesConstants.User},{RolesConstants.Admin}")]
    [HttpDelete("{orderId}")]
    [ProducesResponseType(typeof(CancelOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CancelOrderResult>> CancelOrder(int orderId, string? userId)
    {
        if (!User.IsInRole(RolesConstants.Admin))
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        if (string.IsNullOrEmpty(userId))
        {
            var error = errorFactory.CreateErrorResponse(StatusCodes.Status401Unauthorized);
            return Unauthorized(error);
        }

        var cancelResult = await orderService.CancelOrderAsync(orderId, userId);

        if (!cancelResult.ManageToCancel)
            return BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, cancelResult.Message));

        var response = new CancelOrderResult
        {
            ManageToCancelOrder = true,
            CancelMessage = cancelResult.Message
        };

        if (!string.IsNullOrEmpty(cancelResult.CheckoutSessionId))
        {
            var expiredResult = await checkoutService.ExpireCheckoutSessionAsync(cancelResult.CheckoutSessionId);
            response.ManageToExpireSession = expiredResult.ManageToExpire;
            response.ExpireMessage = expiredResult.Message;
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("delivery-methods")]
    [ProducesResponseType(typeof(IReadOnlyList<DeliveryMethodResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethodResult>>> GetDeliveryMethods() =>
        Ok(await orderService.GetDeliveryMethods());

    [AllowAnonymous]
    [HttpGet("order-status-values")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public ActionResult<string[]> GetOrderStatusValues() =>
        Ok(Enum.GetValues<OrderStatus>());

    [AllowAnonymous]
    [HttpGet("payment-status-values")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public ActionResult<string[]> GetPaymentStatusValues() =>
        Ok(Enum.GetValues<PaymentStatus>());

    #region Admin

    [Authorize(Roles = RolesConstants.Admin)]
    [HttpPut("order-status")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaveResult>> UpdateOrderStatus(UpdateOrderStatusRequest requestData)
    {
        var result = await orderService.UpdateOrderStatusAsync(requestData);
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            StatusCodes.Status404NotFound => NotFound(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            _ => StatusCode(result.StatusCode)
        };
    }

    [Authorize(Roles = RolesConstants.Admin)]
    [HttpPut("payment-status")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaveResult>> UpdatePaymentStatus(UpdatePaymentStatusRequest requestData)
    {
        var result = await orderService.UpdatePaymentStatus(requestData);
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            StatusCodes.Status404NotFound => NotFound(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            _ => StatusCode(result.StatusCode)
        };
    }

    [Authorize(Roles = RolesConstants.Admin)]
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<SaveResult>> GetOrdersCount([FromQuery] OrderSpecsParams specsParams)
    {
        return Ok(await orderService.GetCountAsync(specsParams));
    }

    [Authorize(Roles = RolesConstants.Admin)]
    [HttpGet("order-status-count")]
    [ProducesResponseType(typeof(IReadOnlyList<StatusCountResult<OrderStatus>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StatusCountResult<OrderStatus>>>> GetOrderStatusCount()
    {
        return Ok(await orderService.GetOrderStatusCountAsync());
    }

    [Authorize(Roles = RolesConstants.Admin)]
    [HttpGet("payment-status-count")]
    [ProducesResponseType(typeof(IReadOnlyList<StatusCountResult<OrderStatus>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StatusCountResult<OrderStatus>>>> GetPaymentStatusCount()
    {
        return Ok(await orderService.GetPaymentStatusCountAsync());
    }

    #endregion
}
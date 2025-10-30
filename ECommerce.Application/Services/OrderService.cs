using AutoMapper;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.OrderModule;
using ECommerce.Core.Models.OrderModule.Owned;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.OrderSpecifications;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ECommerce.Application.Services;

public class OrderService(
    ICartService cartService,
    IProductService productService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IConfiguration config,
    IStringLocalizer<OrderService> localizer,
    ICultureService cultureService)
    : IOrderService
{
    public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderInput input, string userId)
    {
        var cart = await cartService.GetCartAsync(input.CartId);
        if (cart is null)
            return new CreateOrderResult() { Message = localizer[L.Order.CartNotFound] };

        if (cart.Items.Count == 0)
            return new CreateOrderResult() { Message = localizer[L.Order.CartEmpty] };

        var deliveryMethod = await unitOfWork
            .Repository<DeliveryMethod>()
            .GetAsync(new DeliveryMethodSpecs(input.DeliveryMethodId));

        if (deliveryMethod is null)
            return new CreateOrderResult() { Message = localizer[L.Order.DeliveryMethodUnavailable] };

        decimal subTotal = 0;
        List<OrderItem> orderItems = [];

        // BEGIN TRANSACTION
        await unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var cartItem in cart.Items)
            {
                var productSpecs = new ProductSpecs(
                    specsParams: new ProductSpecsParams { ProductId = cartItem.ProductId },
                    enablePagination: false,
                    enableSorting: false,
                    enableTracking: true,
                    enableSplittingQuery: true
                );

                productSpecs.IncludeRelatedData(p => p.Translations);

                var product = await unitOfWork
                                  .Repository<Product>()
                                  .GetAsync(productSpecs, checkLocalCache: false)
                              ?? throw new ArgumentException(localizer[L.Order.ProductNotExists, cartItem.ProductName]);


                var maxOrderQty = await productService.GetMaxOrderQuantityAsync(cartItem.ProductId);

                if (cartItem.Quantity > maxOrderQty)
                    throw new ArgumentException(localizer[L.Order.NotEnoughStock, product.Name]);


                product.UnitsInStock -= cartItem.Quantity;

                subTotal += product.Price * cartItem.Quantity;

                orderItems.Add(new OrderItem()
                {
                    Product = new ProductItem()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        PictureUrl = $"{config["BaseUrl"]}/{product.PictureUrl}",
                        NameTranslations =
                            product.Translations.ToDictionary(x => x.LanguageCode.ToString(), x => x.Name)
                    },
                    Price = product.Price,
                    Quantity = cartItem.Quantity
                });
            }

            var order = new Order()
            {
                UserId = userId,
                DeliveryMethodId = input.DeliveryMethodId,
                ShippingAddress = mapper.Map<ShippingAddress>(input.ShippingAddress),
                PaymentMethod = input.PaymentMethod,
                SubTotal = subTotal,
                Items = orderItems,
            };

            unitOfWork.Repository<Order>().Add(order);

            // COMMIT TRANSACTION
            await unitOfWork.CommitTransactionAsync();

            await cartService.DeleteCartAsync(input.CartId);
            return new CreateOrderResult()
            {
                Success = true,
                Message = localizer[L.Order.CreatedSuccessfully],
                CreatedOrderId = order.Id
            };
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new CreateOrderResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<IReadOnlyList<DeliveryMethodResult>> GetDeliveryMethods()
    {
        var canTranslate = cultureService.CanTranslate;
        DeliveryMethodSpecs deliveryMethodSpecs = new DeliveryMethodSpecs(null);
        if (canTranslate) deliveryMethodSpecs.IncludeRelatedData(x => x.Translations);

        var deliveryMethods = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync(deliveryMethodSpecs);
        return mapper.Map<IReadOnlyList<DeliveryMethodResult>>(deliveryMethods);
    }

    public async Task<PaginationResult<OrderResult>> GetOrdersAsync(OrderSpecsParams specsParams)
    {
        var orderSpecs = new OrderSpecs(
            specsParams: specsParams,
            enablePagination: true,
            enableSorting: true,
            enableTracking: false
        );
        orderSpecs.IncludeRelatedData(o => o.Items, o => o.DeliveryMethod, o => o.User);
        var orders = await unitOfWork
            .Repository<Order>()
            .GetAllAsync(orderSpecs);

        var countSpecs = new OrderSpecs(
            specsParams: specsParams,
            enablePagination: false,
            enableSorting: false,
            enableTracking: false
        );
        var count = await unitOfWork
            .Repository<Order>()
            .CountAsync(countSpecs);

        return new PaginationResult<OrderResult>
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Total = count,
            Results = mapper.Map<IReadOnlyList<OrderResult>>(orders)
        };
    }

    public async Task<OrderResult?> GetOrderAsync(int orderId, string? userId)
    {
        var orderSpecs = new OrderSpecs(
            specsParams: new OrderSpecsParams() { OrderId = orderId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false
        );
        orderSpecs.IncludeRelatedData(o => o.Items, o => o.DeliveryMethod, o => o.User);

        var order = await unitOfWork
            .Repository<Order>()
            .GetAsync(orderSpecs);

        if (order is null) return null;

        if (userId is not null && order.UserId != userId) return null;

        return mapper.Map<OrderResult>(order);
    }

    public async Task<int> GetCountAsync(OrderSpecsParams specsParams)
    {
        var countSpecs = new OrderSpecs(
            specsParams: specsParams,
            enablePagination: false,
            enableSorting: false,
            enableTracking: false
        );

        return await unitOfWork
            .Repository<Order>()
            .CountAsync(countSpecs);
    }

    public async Task<OrderCancelationResult> CancelOrderAsync(int orderId, string userId)
    {
        var specs = new OrderSpecs(
            specsParams: new OrderSpecsParams { OrderId = orderId, UserId = userId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true
        );

        specs.IncludeRelatedData(o => o.Items);

        var order = await unitOfWork
            .Repository<Order>()
            .GetAsync(specs, checkLocalCache: false);

        if (order is null || !order.IsCancellable)
            return new OrderCancelationResult { Message = localizer[L.Order.notCancellable] };


        // BEGIN TRANSACTION
        await unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var orderItem in order.Items)
            {
                var productSpecs = new ProductSpecs(
                    specsParams: new ProductSpecsParams { ProductId = orderItem.Product.Id },
                    enablePagination: false,
                    enableSorting: false,
                    enableTracking: true,
                    enableSplittingQuery: false
                );

                var product = await unitOfWork
                                  .Repository<Product>()
                                  .GetAsync(productSpecs, false)
                              ?? throw new ArgumentException(localizer[L.Order.noProductAssociated]);

                product.UnitsInStock += orderItem.Quantity;
            }

            order.OrderStatus = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.PaymentFailed;

            // COMMIT TRANSACTION
            await unitOfWork.CommitTransactionAsync();

            return new OrderCancelationResult
            {
                ManageToCancel = true,
                CheckoutSessionId = order.CheckoutSessionId,
                Message = localizer[L.Order.cancelledSuccessfully]
            };
        }
        catch (Exception ex)
        {
            // ROLLBACK TRANSACTION
            await unitOfWork.RollbackTransactionAsync();
            return new OrderCancelationResult { Message = localizer[L.Order.cancelFailed, ex.Message] };
        }
    }

    public async Task<SaveResult> UpdateOrderStatusAsync(UpdateOrderStatusRequest requestData)
    {
        var specs = new OrderSpecs(
            specsParams: new OrderSpecsParams() { OrderId = requestData.OrderId, },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true
        );

        var order = await unitOfWork
            .Repository<Order>()
            .GetAsync(specs);

        if (order is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "The specified order could not be found."
            };

        if (order.OrderStatus == requestData.NewOrderStatus)
        {
            return new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"The order status is already set to '{requestData.NewOrderStatus}'. No changes were made."
            };
        }

        var oldStatus = order.OrderStatus;

        order.OrderStatus = requestData.NewOrderStatus;

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = $"Order status successfully changed from '{oldStatus}' to '{requestData.NewOrderStatus}'."
            }
            : new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Failed to update the order status. Please try again later."
            };
    }

    public async Task<SaveResult> UpdatePaymentStatus(UpdatePaymentStatusRequest requestData)
    {
        var specs = new OrderSpecs(
            specsParams: new OrderSpecsParams() { OrderId = requestData.OrderId, },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true
        );

        var order = await unitOfWork
            .Repository<Order>()
            .GetAsync(specs);

        if (order is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "The specified order could not be found."
            };

        if (order.PaymentStatus == requestData.NewPaymentStatus)
        {
            return new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message =
                    $"The payment status is already set to '{requestData.NewPaymentStatus}'. No changes were made."
            };
        }

        var oldStatus = order.PaymentStatus;

        order.PaymentStatus = requestData.NewPaymentStatus;

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = $"Payment status successfully changed from '{oldStatus}' to '{requestData.NewPaymentStatus}'."
            }
            : new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Failed to update the payment status. Please try again later."
            };
    }

    public async Task<IReadOnlyList<StatusCountResult<OrderStatus>>> GetOrderStatusCountAsync()
    {
        var query = unitOfWork
            .Repository<Order>()
            .GetAllAsQueryable(null);

        var groupedQuery = query
            .GroupBy(x => x.OrderStatus)
            .Select(g => new StatusCountResult<OrderStatus>()
            {
                Status = g.Key,
                Count = g.Count()
            });

        return await groupedQuery.ToListAsync();
    }

    public async Task<IReadOnlyList<StatusCountResult<PaymentStatus>>> GetPaymentStatusCountAsync()
    {
        var query = unitOfWork
            .Repository<Order>()
            .GetAllAsQueryable(null);

        var groupedQuery = query
            .GroupBy(x => x.PaymentStatus)
            .Select(g => new StatusCountResult<PaymentStatus>()
            {
                Status = g.Key,
                Count = g.Count()
            });

        return await groupedQuery.ToListAsync();
    }
}
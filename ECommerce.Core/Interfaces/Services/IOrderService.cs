using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;
using ECommerce.Core.Dtos.OrderDtos;

namespace ECommerce.Core.Interfaces.Services;

public interface IOrderService
{
    Task<CreateOrderResult> CreateOrderAsync(CreateOrderInput input, string userId);
    Task<IReadOnlyList<DeliveryMethodResult>> GetDeliveryMethods();
    Task<PaginationResult<OrderResult>> GetOrdersAsync(OrderSpecsParams specsParams);
    Task<OrderResult?> GetOrderAsync(int orderId, string? userId);
    Task<int> GetCountAsync(OrderSpecsParams specsParams);
    Task<OrderCancelationResult> CancelOrderAsync(int orderId, string userId);
    Task<SaveResult> UpdateOrderStatusAsync(UpdateOrderStatusRequest requestData);
    Task<SaveResult> UpdatePaymentStatus(UpdatePaymentStatusRequest requestData);
    Task<IReadOnlyList<StatusCountResult<OrderStatus>>> GetOrderStatusCountAsync();
    Task<IReadOnlyList<StatusCountResult<PaymentStatus>>> GetPaymentStatusCountAsync();
}
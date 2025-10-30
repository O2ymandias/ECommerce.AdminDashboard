using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.OrdersDto;

public class UpdateOrderStatusRequest
{
    [Required] [Range(1, int.MaxValue)] public int OrderId { get; set; }
    [Required] public OrderStatus NewOrderStatus { get; set; }
}
using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.OrdersDto;

public class UpdatePaymentStatusRequest
{
    [Required] [Range(1, int.MaxValue)] public int OrderId { get; set; }
    [Required] public PaymentStatus NewPaymentStatus { get; set; }
}
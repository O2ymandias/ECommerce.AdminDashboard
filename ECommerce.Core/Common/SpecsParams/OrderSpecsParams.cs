using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Common.SpecsParams;

public class OrderSpecsParams : BaseSpecsParams
{
    public string? Search { get; set; }
    [Range(1, int.MaxValue)] public int? OrderId { get; set; }
    public string? UserId { get; set; }
    public OrderSortOptions Sort { get; set; } = new();
    public OrderStatus? OrderStatus { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    [Range(1, int.MaxValue)] public int? MinSubtotal { get; set; }
    [Range(1, int.MaxValue)] public int? MaxSubtotal { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
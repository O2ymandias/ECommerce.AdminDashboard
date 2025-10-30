using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.OrderModule.Owned;

namespace ECommerce.Core.Dtos.OrderDtos;

public class OrderResult
{
    public int OrderId { get; set; }
    public string UserEmail { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public decimal DeliveryMethodCost { get; set; }
    public ICollection<OrderItemResult> Items { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public string? CheckoutSessionId { get; set; }
    public bool IsCancellable { get; set; }
}
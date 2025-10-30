using ECommerce.Core.Common.Enums;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.OrderModule.Owned;

namespace ECommerce.Core.Models.OrderModule;

public class Order : ModelBase
{
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    public PaymentMethod PaymentMethod { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public ShippingAddress ShippingAddress { get; set; }

    public DeliveryMethod DeliveryMethod { get; set; }
    public int? DeliveryMethodId { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public ICollection<OrderItem> Items { get; set; }

    public decimal SubTotal { get; set; }

    public string? CheckoutSessionId { get; set; }

    public decimal Total => SubTotal + (DeliveryMethod?.Cost ?? 0);

    public bool IsPaid => PaymentStatus == PaymentStatus.PaymentReceived;

    public bool IsCancellable =>
        (PaymentMethod == PaymentMethod.Cash &&
         (OrderStatus == OrderStatus.Pending || OrderStatus == OrderStatus.Processing)) ||
        (PaymentMethod == PaymentMethod.Online && OrderStatus == OrderStatus.Pending &&
         (PaymentStatus == PaymentStatus.Pending || PaymentStatus == PaymentStatus.AwaitingPayment));
}
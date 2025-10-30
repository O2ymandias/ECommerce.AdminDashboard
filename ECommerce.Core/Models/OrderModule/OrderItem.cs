using ECommerce.Core.Models.OrderModule.Owned;

namespace ECommerce.Core.Models.OrderModule;

public class OrderItem : ModelBase
{
    public ProductItem Product { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Order Order { get; set; }
    public decimal Total => Price * Quantity;
}
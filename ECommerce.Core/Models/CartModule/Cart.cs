namespace ECommerce.Core.Models.CartModule;

public class Cart(string id)
{
    public string Id { get; set; } = id;
    public List<CartItem> Items { get; set; } = [];
}
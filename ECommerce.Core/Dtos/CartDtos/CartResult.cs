namespace ECommerce.Core.Dtos.CartDtos;
public class CartResult
{
	public string Id { get; set; }
	public List<CartItemResult> Items { get; set; }
}

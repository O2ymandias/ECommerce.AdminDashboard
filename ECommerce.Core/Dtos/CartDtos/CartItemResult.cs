namespace ECommerce.Core.Dtos.CartDtos;
public class CartItemResult
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public string ProductPictureUrl { get; set; }
	public decimal ProductPrice { get; set; }
	public int Quantity { get; set; }
}

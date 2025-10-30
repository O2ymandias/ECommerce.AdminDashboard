namespace ECommerce.Core.Models.CartModule;
public class CartItem
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public string ProductPictureUrl { get; set; }
	public decimal ProductPrice { get; set; }
	public int Quantity { get; set; }

	public IDictionary<string, string> NameTranslations { get; set; }
}

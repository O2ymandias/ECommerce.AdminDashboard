using ECommerce.Core.Dtos.ProductDtos;

namespace ECommerce.Core.Dtos.WishlistDtos;
public class WishlistItemResult
{
	public ProductResult Product { get; set; }
	public DateTime CreatedAt { get; set; }
}

using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Core.Models.WishlistModule;
public class WishlistItem : ModelBase
{
	public string UserId { get; set; }
	public AppUser User { get; set; }

	public int ProductId { get; set; }
	public Product Product { get; set; }

	public DateTime CreatedAt { get; set; }
}

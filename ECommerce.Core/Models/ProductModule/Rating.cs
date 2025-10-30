using ECommerce.Core.Models.AuthModule;

namespace ECommerce.Core.Models.ProductModule;
public class Rating : ModelBase
{
	public int Stars { get; set; }
	public string? Title { get; set; }
	public string? Comment { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public int ProductId { get; set; }
	public Product Product { get; set; }

	public string UserId { get; set; }
	public AppUser User { get; set; }
}

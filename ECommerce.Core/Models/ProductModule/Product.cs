using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.CategoryModule;
using ECommerce.Core.Models.WishlistModule;

namespace ECommerce.Core.Models.ProductModule;

public class Product : ModelBase
{
	public string Name { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public string PictureUrl { get; set; }
	public Brand Brand { get; set; }
	public int BrandId { get; set; }

	public Category Category { get; set; }
	public int CategoryId { get; set; }

	public int UnitsInStock { get; set; }

	public ICollection<Rating> Ratings { get; set; } = [];

	public ICollection<ProductTranslation> Translations { get; set; } = [];

	public ICollection<WishlistItem> WishlistItems { get; set; } = [];

	public ICollection<ProductGallery> Galleries { get; set; } = [];
}

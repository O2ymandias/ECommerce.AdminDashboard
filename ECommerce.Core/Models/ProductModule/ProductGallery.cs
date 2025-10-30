namespace ECommerce.Core.Models.ProductModule;

public class ProductGallery : ModelBase
{
	public string PictureUrl { get; set; }
	public string? AltText { get; set; }

	public Product Product { get; set; }
	public int ProductId { get; set; }
}

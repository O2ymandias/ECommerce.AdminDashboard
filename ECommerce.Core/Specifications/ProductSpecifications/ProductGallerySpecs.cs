using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;
public class ProductGallerySpecs : BaseSpecification<ProductGallery>
{
	public ProductGallerySpecs(int productId)
		: base(g => g.ProductId == productId)
	{
		IsTrackingEnabled = false;
		IsPaginationEnabled = false;
		IsSplitQueryEnabled = false;
	}

	public ProductGallerySpecs(int productId, string imagePath)
		: base(g =>
			g.ProductId == productId &&
			g.PictureUrl == imagePath
			)
	{
		IsTrackingEnabled = true;
		IsPaginationEnabled = false;
		IsSplitQueryEnabled = false;
	}
}

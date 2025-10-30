using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;
public class ProductTranslationsSpecs : BaseSpecification<ProductTranslation>
{
	public ProductTranslationsSpecs(int productId)
		: base(x => x.ProductId == productId)
	{
		IsPaginationEnabled = false;
		IsSplitQueryEnabled = false;
		IsTrackingEnabled = false;
	}
}

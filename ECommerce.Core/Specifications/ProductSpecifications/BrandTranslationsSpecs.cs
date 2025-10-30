using ECommerce.Core.Models.BrandModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;

public class BrandTranslationsSpecs : BaseSpecification<BrandTranslation>
{
    public BrandTranslationsSpecs(int brandId)
        : base(x => x.BrandId == brandId)
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = false;
    }
}

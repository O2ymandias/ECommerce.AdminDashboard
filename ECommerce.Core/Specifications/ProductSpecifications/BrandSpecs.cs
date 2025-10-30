using ECommerce.Core.Models.BrandModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;

public class BrandSpecs : BaseSpecification<Brand>
{
    public BrandSpecs()
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = false;
    }

    public BrandSpecs(int brandId) : base(b => b.Id == brandId)
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = true;
    }
}

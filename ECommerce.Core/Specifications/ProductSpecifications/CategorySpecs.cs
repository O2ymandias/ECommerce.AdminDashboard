using ECommerce.Core.Models.CategoryModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;

public class CategorySpecs : BaseSpecification<Category>
{
    public CategorySpecs()
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = false;
    }

    public CategorySpecs(int categoryId)
        : base(c => c.Id == categoryId)
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = true;
    }
}
using ECommerce.Core.Models.CategoryModule;

namespace ECommerce.Core.Specifications.ProductSpecifications;

public class CategoryTranslationSpecs : BaseSpecification<CategoryTranslation>
{
    public CategoryTranslationSpecs(int categoryId)
        : base(x => x.CategoryId == categoryId)
    {
        IsPaginationEnabled = false;
        IsSplitQueryEnabled = false;
        IsTrackingEnabled = false;
    }
}

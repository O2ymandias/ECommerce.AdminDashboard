using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;

namespace ECommerce.Core.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResult>> GetCategoriesAsync();
    Task<CategoryResult?> GetCategoryAsync(int categoryId);
    Task<SaveResult> UpdateCategoryAsync(UpdateBrandOrCategoryRequest requestData);
    Task<SaveResult> CreateCategoryAsync(CreateBrandOrCategoryRequest requestData);
    Task<SaveResult> DeleteCategoryAsync(int categoryId);
    Task<IReadOnlyList<BrandOrCategoryTranslationResult>> GetCategoryTranslationsAsync(int categoryId);
}
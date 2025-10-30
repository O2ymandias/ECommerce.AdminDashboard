using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;

namespace ECommerce.Core.Interfaces.Services;

public interface IBrandService
{
    Task<IReadOnlyList<BrandResult>> GetBrandsAsync();
    Task<BrandResult?> GetBrandAsync(int brandId);
    Task<SaveResult> UpdateBrandAsync(UpdateBrandOrCategoryRequest requestData);
    Task<SaveResult> CreateBrandAsync(CreateBrandOrCategoryRequest requestData);
    Task<SaveResult> DeleteBrandAsync(int brandId);
    Task<IReadOnlyList<BrandOrCategoryTranslationResult>> GetBrandTranslationsAsync(int brandId);
}
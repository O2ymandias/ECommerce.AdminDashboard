using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.ProductDtos;
using ECommerce.Core.Dtos.ProductDtos;
using Microsoft.AspNetCore.Http;


namespace ECommerce.Core.Interfaces.Services;

public interface IProductService
{
    Task<PaginationResult<ProductResult>> GetAllProductsWithCountAsync(ProductSpecsParams specsParams);
    Task<ProductResult?> GetProductByIdAsync(int productId);

    Task<IReadOnlyList<CategoryResult>> GetAllCategoriesAsync();
    Task<IReadOnlyList<ProductGalleryResult>> GetProductGalleryAsync(int productId);
    Task<int> GetMaxOrderQuantityAsync(int productId);


    Task<SaveResult> CreateProductAsync(CreateProductRequest requestData);
    Task<SaveResult> UpdateProductAsync(UpdateProductRequest requestData);
    Task<SaveResult> DeleteProductAsync(int productId);
    Task<bool> DeleteFromProductGalleryAsync(int productId, string imagePath);
    Task<SaveResult> AddToProductGalleryAsync(int productId, IFormFile[] images);
    Task<IReadOnlyList<ProductTranslationResult>> GetProductTranslationsAsync(int productId);
    Task<SaveResult> UpdateProductTranslationsAsync(UpdateProductTranslationsRequest requestData);
}
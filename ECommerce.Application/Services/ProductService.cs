using AutoMapper;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.Options;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.ProductDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.CategoryModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ECommerce.Application.Services;

public class ProductService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<CartOptions> cartConfigOptions,
    ICultureService cultureService,
    IImageUploader imageUploader)
    : IProductService
{
    private const int MaxGalleryImages = 4;
    private readonly CartOptions _cartOptions = cartConfigOptions.Value;

    public async Task<PaginationResult<ProductResult>> GetAllProductsWithCountAsync(ProductSpecsParams specsParams)
    {
        var canTranslate = cultureService.CanTranslate;

        var productSpecs = new ProductSpecs(
            specsParams: specsParams,
            enablePagination: true,
            enableSorting: true,
            enableTracking: false,
            enableSplittingQuery: canTranslate
        );

        productSpecs.IncludeRelatedData(p => p.Brand, p => p.Category);
        if (canTranslate)
        {
            productSpecs.IncludeRelatedData(p => p.Translations);
            productSpecs.IncludeRelatedData(
                p => p.Brand.Translations,
                p => p.Category.Translations
            );
        }

        var countSpecs = new ProductSpecs(
            specsParams: specsParams,
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: false
        );

        var allProducts = await unitOfWork
            .Repository<Product>()
            .GetAllAsync(productSpecs);

        var totalCount = await unitOfWork
            .Repository<Product>()
            .CountAsync(countSpecs);

        return new PaginationResult<ProductResult>
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Results = mapper.Map<IReadOnlyList<ProductResult>>(allProducts),
            Total = totalCount
        };
    }

    public async Task<ProductResult?> GetProductByIdAsync(int productId)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var shouldTranslate = Enum.TryParse(culture.Name, true, out LanguageCode lang) && lang is not LanguageCode.EN;

        var productSpecs = new ProductSpecs(
            new ProductSpecsParams { ProductId = productId },
            false,
            false,
            false,
            shouldTranslate
        );

        productSpecs.IncludeRelatedData(p => p.Brand, p => p.Category);
        if (shouldTranslate)
        {
            productSpecs.IncludeRelatedData(p => p.Translations);
            productSpecs.IncludeRelatedData(
                x => x.Brand.Translations,
                x => x.Category.Translations
            );
        }

        var product = await unitOfWork.Repository<Product>().GetAsync(productSpecs);

        return product is not null ? mapper.Map<ProductResult>(product) : null;
    }

    public async Task<IReadOnlyList<CategoryResult>> GetAllCategoriesAsync()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var shouldTranslate = Enum.TryParse(culture.Name, true, out LanguageCode lang) && lang is not LanguageCode.EN;

        var categorySpecs = new CategorySpecs();

        if (shouldTranslate)
            categorySpecs.IncludeRelatedData(c => c.Translations);

        var categories = await unitOfWork
            .Repository<Category>()
            .GetAllAsync(categorySpecs);

        return mapper.Map<IReadOnlyList<CategoryResult>>(categories);
    }

    public async Task<IReadOnlyList<ProductGalleryResult>> GetProductGalleryAsync(int productId)
    {
        var specs = new ProductGallerySpecs(productId);
        var gallery = await unitOfWork
            .Repository<ProductGallery>()
            .GetAllAsync(specs);
        return mapper.Map<IReadOnlyList<ProductGalleryResult>>(gallery);
    }

    public async Task<int> GetMaxOrderQuantityAsync(int productId)
    {
        var specs = new ProductSpecs(
            new ProductSpecsParams { ProductId = productId },
            false,
            false,
            false,
            false
        );

        var product = await unitOfWork
                          .Repository<Product>()
                          .GetAsync(specs)
                      ?? throw new ArgumentException($"There is no product with id `{productId}`");

        var maxOrder = (int)Math.Ceiling(product.UnitsInStock * _cartOptions.MaxOrderRate);
        return Math.Min(maxOrder, _cartOptions.MaxOrderQuantityCap);
    }

    public async Task<SaveResult> UpdateProductAsync(UpdateProductRequest requestData)
    {
        var specs = new ProductSpecs(
            specsParams: new ProductSpecsParams { ProductId = requestData.Id },
            enablePagination: false,
            enableSorting: false,
            enableSplittingQuery: false,
            enableTracking: true
        );

        specs.IncludeRelatedData(p => p.Brand, p => p.Category);

        var product = await unitOfWork
            .Repository<Product>()
            .GetAsync(specs);

        if (product is null) return new SaveResult { Message = "Product not found" };

        if (requestData.Image is not null)
        {
            var uploadResult = await imageUploader.UploadImageAsync(requestData.Image, "images/products");
            if (!uploadResult.Uploaded || uploadResult.FilePath is null)
                return new SaveResult
                {
                    Success = false,
                    Message = uploadResult.ErrorMessage ?? "Unable to upload the product image"
                };

            imageUploader.DeleteFile(product.PictureUrl);
            product.PictureUrl = uploadResult.FilePath;
        }

        product.Name = requestData.Name;
        product.Description = requestData.Description;
        product.Price = requestData.Price;
        product.UnitsInStock = requestData.UnitsInStock;
        product.BrandId = requestData.BrandId;
        product.CategoryId = requestData.CategoryId;

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult
            {
                Success = true,
                Message = "Product updated successfully."
            }
            : new SaveResult
            {
                Success = false,
                Message = "No changes detected. The product may already have the specified values."
            };
    }

    public async Task<bool> DeleteFromProductGalleryAsync(int productId, string imagePath)
    {
        var specs = new ProductGallerySpecs(productId, imagePath);

        var productGalleryImage = await unitOfWork
            .Repository<ProductGallery>()
            .GetAsync(specs);

        if (productGalleryImage is null)
            return false;

        unitOfWork
            .Repository<ProductGallery>()
            .Delete(productGalleryImage);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        if (rowsAffected == 0)
            return false;

        imageUploader.DeleteFile(imagePath);
        return true;
    }

    public async Task<SaveResult> AddToProductGalleryAsync(int productId, IFormFile[] images)
    {
        if (images.Length == 0)
            return new SaveResult
            {
                Success = false,
                Message = "No images were uploaded."
            };

        var specs = new ProductGallerySpecs(productId);

        var productGalleryCount = await unitOfWork
            .Repository<ProductGallery>()
            .CountAsync(specs);


        if ((images.Length + productGalleryCount) > MaxGalleryImages)
        {
            return new SaveResult
            {
                Success = false,
                Message = $"You can upload a maximum of  {MaxGalleryImages - productGalleryCount} more images."
            };
        }


        if (images.Length > MaxGalleryImages)
            return new SaveResult
            {
                Success = false,
                Message = $"You can upload a maximum of  {MaxGalleryImages} images."
            };

        List<string> imagePaths = [];
        string? error = null;
        var allImagesUploaded = true;

        foreach (var image in images)
        {
            var uploadResult = await imageUploader.UploadImageAsync(image, "images/products/gallery");
            if (uploadResult.Uploaded && uploadResult.FilePath is not null)
            {
                imagePaths.Add(uploadResult.FilePath);
            }
            else
            {
                allImagesUploaded = false;
                error = uploadResult.ErrorMessage;
                break;
            }
        }

        if (!allImagesUploaded)
        {
            imagePaths.ForEach(path => imageUploader.DeleteFile(path));
            return new SaveResult
            {
                Success = false,
                Message = error ?? "Unable to upload the gallery images"
            };
        }

        unitOfWork
            .Repository<ProductGallery>()
            .AddRange(
                imagePaths.Select(path => new ProductGallery()
                {
                    ProductId = productId,
                    PictureUrl = path
                })
            );

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            imagePaths.ForEach(path => imageUploader.DeleteFile(path));
            return new SaveResult
            {
                Success = false,
                Message = "Unable to add the gallery images to the product"
            };
        }

        return new SaveResult
        {
            Success = true,
            Message = "Gallery images added successfully."
        };
    }

    public async Task<IReadOnlyList<ProductTranslationResult>> GetProductTranslationsAsync(int productId)
    {
        var specs = new ProductTranslationsSpecs(productId);
        var translations = await unitOfWork
            .Repository<ProductTranslation>()
            .GetAllAsync(specs);

        IReadOnlyList<ProductTranslationResult> result =
        [
            .. translations.Select(t => new ProductTranslationResult
            {
                LanguageCode = t.LanguageCode,
                Description = t.Description,
                Name = t.Name
            })
        ];

        return result;
    }

    public async Task<SaveResult> UpdateProductTranslationsAsync(UpdateProductTranslationsRequest requestData)
    {
        var specs = new ProductSpecs(
            specsParams: new ProductSpecsParams { ProductId = requestData.ProductId },
            enablePagination: false,
            enableSorting: false,
            enableSplittingQuery: false,
            enableTracking: true
        );

        specs.IncludeRelatedData(p => p.Translations);

        var product = await unitOfWork
            .Repository<Product>()
            .GetAsync(specs);

        if (product is null)
            return new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Product not found."
            };

        var translation = product.Translations.FirstOrDefault(t => t.LanguageCode == requestData.LanguageCode);

        if (translation is null)
        {
            unitOfWork
                .Repository<ProductTranslation>()
                .Add(new ProductTranslation()
                {
                    ProductId = requestData.ProductId,
                    Name = requestData.Name,
                    Description = requestData.Description,
                    LanguageCode = requestData.LanguageCode
                });
        }
        else
        {
            translation.Name = requestData.Name;
            translation.Description = requestData.Description;
        }

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Product translations updated successfully."
            }
            : new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "No changes were saved. Ensure the provided data is different from the existing translations."
            };
    }

    public async Task<SaveResult> CreateProductAsync(CreateProductRequest requestData)
    {
        var product = new Product()
        {
            Name = requestData.Name,
            Description = requestData.Description,
            Price = requestData.Price,
            UnitsInStock = requestData.UnitsInStock,
            BrandId = requestData.BrandId,
            CategoryId = requestData.CategoryId
        };


        var uploadImageResult = await imageUploader.UploadImageAsync(requestData.Image, "images/products");

        if (uploadImageResult.Uploaded && uploadImageResult.FilePath is not null)
        {
            product.PictureUrl = uploadImageResult.FilePath;
        }
        else
        {
            return new SaveResult
            {
                Success = false,
                Message = uploadImageResult.ErrorMessage ?? "Unable to upload the product image"
            };
        }


        unitOfWork
            .Repository<Product>()
            .Add(product);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Product created successfully."
            }
            : new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Unable to create the product."
            };
    }

    public async Task<SaveResult> DeleteProductAsync(int productId)
    {
        var specs = new ProductSpecs(
            specsParams: new ProductSpecsParams { ProductId = productId },
            enablePagination: false,
            enableSorting: false,
            enableSplittingQuery: false,
            enableTracking: true
        );

        specs.IncludeRelatedData(p => p.Galleries);

        var product = await unitOfWork
            .Repository<Product>()
            .GetAsync(specs);

        if (product is null)
            return new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Product not found."
            };

        unitOfWork
            .Repository<Product>()
            .Delete(product);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        if (rowsAffected <= 0)
        {
            return new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Unable to delete the product."
            };
        }

        imageUploader.DeleteFile(product.PictureUrl);

        foreach (var galleryImage in product.Galleries)
        {
            imageUploader.DeleteFile(galleryImage.PictureUrl);
        }

        return new SaveResult
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Product deleted successfully."
        };
    }
}
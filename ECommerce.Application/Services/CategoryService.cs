using AutoMapper;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.CategoryModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IImageUploader imageUploader) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryResult>> GetCategoriesAsync()
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

    public async Task<CategoryResult?> GetCategoryAsync(int categoryId)
    {
        var specs = new CategorySpecs(categoryId);

        var category = await unitOfWork
            .Repository<Category>()
            .GetAsync(specs);

        return category is null
            ? null
            : mapper.Map<CategoryResult>(category);
    }

    public async Task<SaveResult> CreateCategoryAsync(CreateBrandOrCategoryRequest requestData)
    {
        var uploadImage = await imageUploader.UploadImageAsync(requestData.Image, "images/categories");
        if (!uploadImage.Uploaded || uploadImage.FilePath is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Image upload failed."
            };

        var pictureUrl = uploadImage.FilePath;


        if (requestData.Translations.Count == 0)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "No translations detected."
            };

        var translations = requestData.Translations
            .Select(translation => new CategoryTranslation()
            {
                Name = translation.Name,
                LanguageCode = translation.LanguageCode,
            })
            .ToList();


        var category = new Category()
        {
            Name = requestData.Name,
            PictureUrl = pictureUrl,
            Translations = translations,
        };

        unitOfWork
            .Repository<Category>()
            .Add(category);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Category added successfully."
            }
            : new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Unable to create category. try again later."
            };
    }

    public async Task<SaveResult> UpdateCategoryAsync(UpdateBrandOrCategoryRequest requestData)
    {
        var specs = new CategorySpecs(requestData.Id);
        specs.IncludeRelatedData(c => c.Translations);

        var category = await unitOfWork
            .Repository<Category>()
            .GetAsync(specs);

        if (category is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Category not found."
            };

        if (requestData.Image is not null)
        {
            var uploadResult = await imageUploader.UploadImageAsync(requestData.Image, "images/categories");
            if (!uploadResult.Uploaded)
                return new SaveResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Image upload failed."
                };

            imageUploader.DeleteFile(category.PictureUrl);
            category.PictureUrl = uploadResult.FilePath;
        }

        category.Name = requestData.Name;

        foreach (var translation in requestData.Translations)
        {
            var categoryTrans = category.Translations.FirstOrDefault(t => t.LanguageCode == translation.LanguageCode);
            if (categoryTrans is null)
            {
                category.Translations.Add(new CategoryTranslation()
                {
                    Name = translation.Name,
                    LanguageCode = translation.LanguageCode,
                });
            }
            else
            {
                categoryTrans.Name = translation.Name;
            }
        }

        var rowsAffected = await unitOfWork
            .SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Category updated successfully."
            }
            : new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "No changes detected. The category may already have the specified values."
            };
    }

    public async Task<SaveResult> DeleteCategoryAsync(int categoryId)
    {
        var specs = new CategorySpecs(categoryId);
        var category = await unitOfWork
            .Repository<Category>()
            .GetAsync(specs);

        if (category is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Category not found."
            };

        // Getting The Associated Products so we can delete their (Images and Galleries).
        var productSpecs = new ProductSpecs(
            specsParams: new ProductSpecsParams() { CategoryId = categoryId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: true
        );
        productSpecs.IncludeRelatedData(c => c.Galleries);

        var products = await unitOfWork
            .Repository<Product>()
            .GetAllAsync(productSpecs);

        var productsImagesPaths = products
            .Select(p => p.PictureUrl)
            .ToList();

        var productsGalleriesPaths = products
            .SelectMany(p => p.Galleries)
            .Select(g => g.PictureUrl)
            .ToList();

        unitOfWork
            .Repository<Category>()
            .Delete(category);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        if (rowsAffected <= 0)
        {
            return new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Category deleted successfully."
            };
        }

        imageUploader.DeleteFile(category.PictureUrl);
        productsImagesPaths.ForEach(imageUploader.DeleteFile);
        productsGalleriesPaths.ForEach(imageUploader.DeleteFile);

        return new SaveResult()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Category deleted successfully."
        };
    }

    public async Task<IReadOnlyList<BrandOrCategoryTranslationResult>> GetCategoryTranslationsAsync(int categoryId)
    {
        var specs = new CategoryTranslationSpecs(categoryId);
        var translations = await unitOfWork
            .Repository<CategoryTranslation>()
            .GetAllAsync(specs);

        IReadOnlyList<BrandOrCategoryTranslationResult> result =
        [
            .. translations.Select(b => new BrandOrCategoryTranslationResult()
            {
                LanguageCode = b.LanguageCode,
                Name = b.Name,
            })
        ];

        return result;
    }
}
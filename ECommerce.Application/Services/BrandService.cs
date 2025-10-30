using AutoMapper;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Services;

public class BrandService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IImageUploader imageUploader) : IBrandService
{
    public async Task<IReadOnlyList<BrandResult>> GetBrandsAsync()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var shouldTranslate = Enum.TryParse(culture.Name, true, out LanguageCode lang) && lang is not LanguageCode.EN;

        var brandSpecs = new BrandSpecs();
        if (shouldTranslate)
            brandSpecs.IncludeRelatedData(b => b.Translations);

        var brands = await unitOfWork
            .Repository<Brand>()
            .GetAllAsync(brandSpecs);

        return mapper.Map<IReadOnlyList<BrandResult>>(brands);
    }

    public async Task<BrandResult?> GetBrandAsync(int brandId)
    {
        var specs = new BrandSpecs(brandId);

        var brand = await unitOfWork
            .Repository<Brand>()
            .GetAsync(specs);

        return brand is null
            ? null
            : mapper.Map<BrandResult>(brand);
    }

    public async Task<SaveResult> CreateBrandAsync(CreateBrandOrCategoryRequest requestData)
    {
        var uploadImage = await imageUploader.UploadImageAsync(requestData.Image, "images/brands");
        if (!uploadImage.Uploaded || uploadImage.FilePath is null)
        {
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Image upload failed."
            };
        }

        var pictureUrl = uploadImage.FilePath;

        if (requestData.Translations.Count == 0)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "No translations detected."
            };

        var translations = requestData.Translations
            .Select(translation => new BrandTranslation()
            {
                Name = translation.Name,
                LanguageCode = translation.LanguageCode,
            })
            .ToList();

        var brand = new Brand()
        {
            Name = requestData.Name,
            PictureUrl = pictureUrl,
            Translations = translations,
        };

        unitOfWork
            .Repository<Brand>()
            .Add(brand);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Brand added successfully."
            }
            : new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Unable to create brand. try again later."
            };
    }

    public async Task<SaveResult> UpdateBrandAsync(UpdateBrandOrCategoryRequest requestData)
    {
        var specs = new BrandSpecs(requestData.Id);
        specs.IncludeRelatedData(b => b.Translations);

        var brand = await unitOfWork
            .Repository<Brand>()
            .GetAsync(specs);

        if (brand is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Brand not found."
            };

        if (requestData.Image is not null)
        {
            var uploadResult = await imageUploader.UploadImageAsync(requestData.Image, "images/brands");
            if (!uploadResult.Uploaded)
                return new SaveResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Image upload failed."
                };

            imageUploader.DeleteFile(brand.PictureUrl);
            brand.PictureUrl = uploadResult.FilePath;
        }

        brand.Name = requestData.Name;

        foreach (var translation in requestData.Translations)
        {
            var brandTrans = brand.Translations.FirstOrDefault(t => t.LanguageCode == translation.LanguageCode);
            if (brandTrans is null)
            {
                brand.Translations.Add(new BrandTranslation()
                {
                    Name = translation.Name,
                    LanguageCode = translation.LanguageCode,
                });
            }
            else
            {
                brandTrans.Name = translation.Name;
            }
        }

        var rowsAffected = await unitOfWork
            .SaveChangesAsync();

        return rowsAffected > 0
            ? new SaveResult
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Brand updated successfully."
            }
            : new SaveResult
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "No changes detected. The brand may already have the specified values."
            };
    }

    public async Task<SaveResult> DeleteBrandAsync(int brandId)
    {
        var specs = new BrandSpecs(brandId);
        var brand = await unitOfWork
            .Repository<Brand>()
            .GetAsync(specs);

        if (brand is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Brand not found."
            };

        // Getting The Associated Products so we can delete their (Images and Galleries).
        var productSpecs = new ProductSpecs(
            specsParams: new ProductSpecsParams() { BrandId = brandId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: true
        );
        productSpecs.IncludeRelatedData(b => b.Galleries);

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
            .Repository<Brand>()
            .Delete(brand);

        var rowsAffected = await unitOfWork.SaveChangesAsync();

        if (rowsAffected <= 0)
        {
            return new SaveResult()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Brand deleted successfully."
            };
        }

        imageUploader.DeleteFile(brand.PictureUrl);
        productsImagesPaths.ForEach(imageUploader.DeleteFile);
        productsGalleriesPaths.ForEach(imageUploader.DeleteFile);

        return new SaveResult()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Brand deleted successfully."
        };
    }

    public async Task<IReadOnlyList<BrandOrCategoryTranslationResult>> GetBrandTranslationsAsync(int brandId)
    {
        var specs = new BrandTranslationsSpecs(brandId);
        var translations = await unitOfWork
            .Repository<BrandTranslation>()
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
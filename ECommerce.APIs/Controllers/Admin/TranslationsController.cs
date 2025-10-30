using System.ComponentModel.DataAnnotations;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.DashboardDtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers.Admin;

public class TranslationsController(
    IProductService productService,
    IBrandService brandService,
    ICategoryService categoryService,
    IApiErrorResponseFactory errorFactory)
    : AdminController
{
    [HttpGet("keys")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public ActionResult<string[]> GetTranslationKeys()
    {
        var keys = Enum.GetNames<LanguageCode>();
        return Ok(keys);
    }


    [HttpGet("products/{productId}")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductTranslationResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductTranslationResult>>> GetProductTranslations(
        [Range(1, int.MaxValue)] int productId)
    {
        return Ok(await productService.GetProductTranslationsAsync(productId));
    }

    [HttpPost("products")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaveResult>> UpdateProductTranslations(UpdateProductTranslationsRequest requestData)
    {
        var result = await productService.UpdateProductTranslationsAsync(requestData);
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            StatusCodes.Status404NotFound => NotFound(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            _ => StatusCode(result.StatusCode)
        };
    }


    [HttpGet("brands/{brandId}")]
    [ProducesResponseType(typeof(IReadOnlyList<BrandOrCategoryTranslationResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BrandOrCategoryTranslationResult>>> GetBrandTranslations(
        [Range(1, int.MaxValue)] int brandId)
    {
        var translations = await brandService.GetBrandTranslationsAsync(brandId);
        return Ok(translations);
    }

    [HttpGet("categories/{categoryId}")]
    [ProducesResponseType(typeof(IReadOnlyList<BrandOrCategoryTranslationResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BrandOrCategoryTranslationResult>>> GetCategoryTranslations(
        [Range(1, int.MaxValue)] int categoryId)
    {
        var translations = await categoryService.GetCategoryTranslationsAsync(categoryId);
        return Ok(translations);
    }
}
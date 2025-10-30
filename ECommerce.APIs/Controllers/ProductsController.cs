using System.ComponentModel.DataAnnotations;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.DashboardDtos.ProductDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductService productService,
    IBrandService brandService,
    IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    // [TypeFilter(typeof(CacheAttribute<PaginationResult<ProductResult>>), Arguments = [15])]
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<ProductResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginationResult<ProductResult>>>
        GetAll([FromQuery] ProductSpecsParams specsParams)
    {
        return Ok(await productService.GetAllProductsWithCountAsync(specsParams));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResult>> Get([FromRoute] int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product is null
            ? NotFound(errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound))
            : Ok(product);
    }


    [HttpGet("gallery/{productId}")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductGalleryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductGalleryResult>>> GetProductGallery(int productId) =>
        Ok(await productService.GetProductGalleryAsync(productId));

    [HttpGet("{productId}/max-order-quantity")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetMaxOrderQuantity(int productId) =>
        Ok(await productService.GetMaxOrderQuantityAsync(productId));


    #region Admin

    [HttpPost]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> CreateProduct(CreateProductRequest requestData)
    {
        var result = await productService.CreateProductAsync(requestData);
        return result.Success
            ? Ok(result)
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }


    [HttpPut]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> UpdateProduct(UpdateProductRequest requestData)
    {
        var result = await productService.UpdateProductAsync(requestData);
        return result.Success
            ? Ok(result)
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }

    [HttpDelete("gallery")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeleteFromProductGallery(
        [FromQuery] DeleteFromProductGalleryRequest requestData) =>
        Ok(await productService.DeleteFromProductGalleryAsync(requestData.ProductId, requestData.ImagePath));

    [HttpPost("gallery")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> AddToProductGallery([FromForm] AddToProductGalleryRequest requestData)
    {
        var result = await productService.AddToProductGalleryAsync(requestData.ProductId, requestData.Images);
        return result.Success
            ? Ok(result)
            : BadRequest(errorFactory.CreateErrorResponse(StatusCodes.Status400BadRequest, result.Message));
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeleteProduct([FromRoute] int id)
    {
        var result = await productService.DeleteProductAsync(id);
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

    [HttpPut("brands")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> UpdateBrand([FromForm] UpdateBrandOrCategoryRequest requestData)
    {
        var result = await brandService.UpdateBrandAsync(requestData);

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

    [HttpPost("brands")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> CreateBrand([FromForm] CreateBrandOrCategoryRequest requestData)
    {
        var result = await brandService.CreateBrandAsync(requestData);
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(
                errorFactory.CreateErrorResponse(result.StatusCode, result.Message)),
            _ => StatusCode(result.StatusCode)
        };
    }

    [HttpDelete("brands/{brandId}")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaveResult>> DeleteBrand([Required] [Range(1, int.MaxValue)] int brandId)
    {
        var result = await brandService.DeleteBrandAsync(brandId);
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

    #endregion
}
using System.ComponentModel.DataAnnotations;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController(IBrandService brandService, IApiErrorResponseFactory errorFactory) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<BrandResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<BrandResult>>> GetBrands()
        {
            var brands = await brandService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BrandResult>> GetBrandById(int id)
        {
            var brand = await brandService.GetBrandAsync(id);
            return brand is null
                ? NotFound(errorFactory.CreateErrorResponse(StatusCodes.Status404NotFound, "Brand not found"))
                : Ok(brand);
        }

        [HttpPost]
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

        [HttpPut]
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

        [HttpDelete("{brandId}")]
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
    }
}
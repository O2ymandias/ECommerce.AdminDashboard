using System.ComponentModel.DataAnnotations;
using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(
    ICategoryService categoryService,
    IApiErrorResponseFactory errorFactory)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryResult>>> GetCategories()
    {
        var categories = await categoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResult>> GetCategoryById(int id)
    {
        var category = await categoryService.GetCategoryAsync(id);
        return category is null
            ? NotFound()
            : Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveResult>> CreateCategory([FromForm] CreateBrandOrCategoryRequest requestData)
    {
        var result = await categoryService.CreateCategoryAsync(requestData);
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
    public async Task<ActionResult<SaveResult>> UpdateCategory([FromForm] UpdateBrandOrCategoryRequest requestData)
    {
        var result = await categoryService.UpdateCategoryAsync(requestData);

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

    [HttpDelete("{categoryId}")]
    [ProducesResponseType(typeof(SaveResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaveResult>> DeleteCategory([Required] [Range(1, int.MaxValue)] int categoryId)
    {
        var result = await categoryService.DeleteCategoryAsync(categoryId);
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
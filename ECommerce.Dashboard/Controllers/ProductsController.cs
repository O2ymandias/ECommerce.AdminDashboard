using ECommerce.Core.Common.Options;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;
using ECommerce.Dashboard.Models.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ECommerce.Dashboard.Controllers;
public class ProductsController : Controller
{
	private readonly IProductService _productService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IConfiguration _config;
	private readonly HttpClient _httpClient;
	private readonly IAuthService _authService;
	private readonly AdminOptions _adminOptions;
	private readonly ImageUploaderOptions _imageUploaderOptions;

	public ProductsController(
		IProductService productService,
		IUnitOfWork unitOfWork,
		IConfiguration config,
		IHttpClientFactory httpClientFactory,
		IAuthService authService,
		IOptions<ImageUploaderOptions> imageUploaderOptions,
		IOptions<AdminOptions> adminOptions
		)
	{
		_productService = productService;
		_unitOfWork = unitOfWork;
		_config = config;
		_httpClient = httpClientFactory.CreateClient("MyApi");
		_authService = authService;
		_adminOptions = adminOptions.Value;
		_imageUploaderOptions = imageUploaderOptions.Value;
	}


	public async Task<ActionResult> Index(ProductSpecsParams specsParams)
	{
		var products = await _productService.GetAllProductsWithCountAsync(specsParams);
		return View(products);
	}

	public async Task<ActionResult> Details(int id)
	{
		var product = await _productService.GetProductByIdAsync(id);

		if (product is null) return NotFound();

		return View(product);
	}

	[HttpGet]
	public async Task<ActionResult> Update(int id)
	{
		var specs = new ProductSpecs(
			specsParams: new ProductSpecsParams { ProductId = id },
			enablePagination: false,
			enableSorting: false,
			enableTracking: false,
			enableSplittingQuery: false
			);

		var product = await _unitOfWork
			.Repository<Product>()
			.GetAsync(specs);

		if (product is null) return NotFound();

		var updateProductViewModel = new UpdateProductViewModel()
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			UnitsInStock = product.UnitsInStock,
			BrandId = product.BrandId,
			CategoryId = product.CategoryId,
			PictureUrl = $"{_config["BaseUrl"]}/{product.PictureUrl}"
		};

		return View(updateProductViewModel);
	}

	[HttpPost]
	public async Task<ActionResult> Update([FromRoute] int id, UpdateProductViewModel model)
	{
		if (id != model.Id) return BadRequest();

		if (!ModelState.IsValid) return View(model);

		if (model.Picture is not null)
		{
			var isValid = ValidateImageAsync(model.Picture);

			if (!isValid) return View(model);

			using var content = new MultipartFormDataContent();
			using var fileContent = new StreamContent(model.Picture.OpenReadStream());
			content.Add(fileContent, "image", model.Picture.FileName);

			var loginResult = await _authService.LoginUserAsync(new()
			{
				UserNameOrEmail = _adminOptions.UserName,
				Password = _adminOptions.Password
			});

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

			var uploaded = await _httpClient.PostAsync($"ImageUploader/upload-image?folderName=images/products", content);
			if (!uploaded.IsSuccessStatusCode)
			{
				ModelState.AddModelError(
					nameof(UpdateProductViewModel.Picture),
					"Something went wrong while uploading the image. Please try again later."
					);
				return View(model);
			}
			var responseData = await uploaded.Content.ReadFromJsonAsync<Dictionary<string, string>>();
			if (
				responseData is not null &&
				responseData.TryGetValue("filePath", out var filePath) &&
				filePath is not null
				)
			{
				var oldPictureUrl = model.PictureUrl.Replace(_config["BaseUrl"] + "/", string.Empty);
				var deleteResponse = await _httpClient.DeleteAsync($"ImageUploader/delete-image?filePath={oldPictureUrl}");

				model.PictureUrl = filePath;
			}
		}

		var specs = new ProductSpecs(
			specsParams: new ProductSpecsParams { ProductId = id },
			enablePagination: false,
			enableSorting: false,
			enableTracking: true,
			enableSplittingQuery: false
			);

		var product = await _unitOfWork
			.Repository<Product>()
			.GetAsync(specs);

		if (product is null) return NotFound();

		var updated = UpdateProduct(model, product);

		if (!updated)
		{
			TempData["InfoMessage"] = "No changes detected. Product not updated.";
			return RedirectToAction(nameof(Details), new { id = product.Id });
		}

		_unitOfWork.Repository<Product>().Update(product);
		var rowsAffected = await _unitOfWork.SaveChangesAsync();

		if (rowsAffected == 0)
		{
			ModelState.AddModelError("", "Something went wrong while updating the product. Please try again later.");
			return View(model);
		}

		TempData["SuccessMessage"] = "Product updated successfully.";

		return RedirectToAction(nameof(Details), new { id = product.Id });
	}

	private bool ValidateImageAsync(IFormFile Picture)
	{
		var allowedExtensions = _imageUploaderOptions.AllowedExtensions;
		var maxFileSize = _imageUploaderOptions.MaxFileSizeMB * 1024 * 1024;

		if (!allowedExtensions.Contains(Path.GetExtension(Picture.FileName).ToLower()))
		{
			ModelState.AddModelError(
				nameof(UpdateProductViewModel.Picture),
				$"Invalid file type. Only {string.Join(", ", allowedExtensions)} files are allowed."
				);

			return false;
		}

		if (Picture.Length == 0)
		{
			ModelState.AddModelError(nameof(UpdateProductViewModel.Picture), "The file is empty.");
			return false;
		}

		if (Picture.Length > maxFileSize)
		{
			ModelState.AddModelError(
				nameof(UpdateProductViewModel.Picture),
				$"File size exceeds the maximum limit of {_imageUploaderOptions.MaxFileSizeMB} MB."
				);
			return false;
		}
		return true;
	}

	private static bool UpdateProduct(UpdateProductViewModel model, Product product)
	{
		bool hasChanges = false;

		if (product.Name != model.Name)
		{
			product.Name = model.Name;
			hasChanges = true;
		}
		if (product.Description != model.Description)
		{
			product.Description = model.Description;
			hasChanges = true;
		}
		if (product.Price != model.Price)
		{
			product.Price = model.Price;
			hasChanges = true;
		}
		if (product.UnitsInStock != model.UnitsInStock)
		{
			product.UnitsInStock = model.UnitsInStock;
			hasChanges = true;
		}
		if (product.BrandId != model.BrandId)
		{
			product.BrandId = model.BrandId;
			hasChanges = true;
		}
		if (product.CategoryId != model.CategoryId)
		{
			product.CategoryId = model.CategoryId;
			hasChanges = true;
		}
		if (product.PictureUrl != model.PictureUrl)
		{
			product.PictureUrl = model.PictureUrl;
			hasChanges = true;
		}
		return hasChanges;
	}
}

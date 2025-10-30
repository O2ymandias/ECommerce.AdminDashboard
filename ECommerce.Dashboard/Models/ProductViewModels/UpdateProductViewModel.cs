using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.Models.ProductViewModels;

public class UpdateProductViewModel
{
	[Required]
	public int Id { get; set; }

	[Required]
	[StringLength(50, MinimumLength = 3)]
	public string Name { get; set; }

	[Required]
	[MinLength(15)]
	public string Description { get; set; }

	[Required]
	[Range(0.1, double.MaxValue, ErrorMessage = "The Amount Is Invalid, Minimum Amount Is $0.1")]
	public decimal Price { get; set; }

	[Required]
	[Range(0, int.MaxValue, ErrorMessage = "The Stock Is Invalid, Minimum Stock Is 0")]
	[DisplayName("Units In Stock")]
	public int UnitsInStock { get; set; }

	[Required(ErrorMessage = "Brand Is Required"), DisplayName("Brand")]
	public int BrandId { get; set; }

	[Required(ErrorMessage = "Category Is Required"), DisplayName("Category")]
	public int CategoryId { get; set; }

	[Required]
	public string PictureUrl { get; set; }
	public IFormFile? Picture { get; set; }
}

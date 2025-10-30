using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.RatingDtos;
public class RatingInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[Range(1, int.MaxValue, ErrorMessage = L.Validation.Range)]
	public int ProductId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[Range(1, 5, ErrorMessage = L.Validation.Range)]
	public int Stars { get; set; }

	[MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
	public string? Title { get; set; }

	[MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
	public string? Comment { get; set; }
}

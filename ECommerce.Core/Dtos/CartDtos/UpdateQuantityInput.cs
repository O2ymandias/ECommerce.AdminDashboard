using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.CartDtos;
public class UpdateQuantityInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string CartId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public int ProductId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public int NewQuantity { get; set; }
}

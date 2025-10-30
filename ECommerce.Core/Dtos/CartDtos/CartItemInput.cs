using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.CartDtos;
public class CartItemInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string CartId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[Range(1, int.MaxValue, ErrorMessage = L.Validation.Range)]
	public int ProductId { get; set; }

}

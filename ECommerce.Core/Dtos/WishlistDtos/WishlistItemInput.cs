using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.WishlistDtos;
public class WishlistItemInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[Range(1, int.MaxValue, ErrorMessage = L.Validation.Range)]
	public int ProductId { get; set; }

}

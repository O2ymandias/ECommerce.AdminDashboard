using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class AddressInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string Street { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string City { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string Country { get; set; }
}

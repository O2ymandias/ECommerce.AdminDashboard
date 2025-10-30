using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.ProfileDtos;
public class EmailChangeInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[EmailAddress(ErrorMessage = L.Validation.Email)]
	public string NewEmail { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public string Password { get; set; }
}

using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class ForgetPasswordInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[EmailAddress(ErrorMessage = L.Validation.Email)]
	public string Email { get; set; }
}

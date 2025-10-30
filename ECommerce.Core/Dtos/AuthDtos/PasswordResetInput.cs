using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class PasswordResetInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[EmailAddress(ErrorMessage = L.Validation.Email)]
	public string Email { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public string Token { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[RegularExpression(
		@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
		ErrorMessage = L.Validation.PasswordPolicy)]
	public string NewPassword { get; set; }
}

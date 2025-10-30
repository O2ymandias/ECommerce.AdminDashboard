using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class PasswordChangeInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string CurrentPassword { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[RegularExpression(
	@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
	ErrorMessage = L.Validation.PasswordPolicy)]
	public string NewPassword { get; set; }

}

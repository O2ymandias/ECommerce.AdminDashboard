using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class RegisterInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string DisplayName { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[EmailAddress(ErrorMessage = L.Validation.Email)]
	public string Email { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string UserName { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[RegularExpression(
		@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
		ErrorMessage = L.Validation.PasswordPolicy)]
	public string Password { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public AddressInput Address { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = L.Validation.InvalidPhoneEgypt)]
	public string PhoneNumber { get; set; }

}

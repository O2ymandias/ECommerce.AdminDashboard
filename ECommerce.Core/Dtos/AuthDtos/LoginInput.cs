using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.AuthDtos;
public class LoginInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string UserNameOrEmail { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public string Password { get; set; }

}

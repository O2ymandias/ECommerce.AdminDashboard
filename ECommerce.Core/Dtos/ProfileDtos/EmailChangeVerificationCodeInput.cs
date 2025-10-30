using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.ProfileDtos;
public class EmailChangeVerificationCodeInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string VerificationCode { get; set; }
}

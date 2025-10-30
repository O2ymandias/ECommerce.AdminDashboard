using ECommerce.Core.Common.Constants;
using ECommerce.Core.Dtos.AuthDtos;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.ProfileDtos;
public class BasicInfoUpdateInput
{

	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string DisplayName { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[Phone(ErrorMessage = L.Validation.Phone)]
	[RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = L.Validation.InvalidPhoneEgypt)]
	public string PhoneNumber { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public AddressInput Address { get; set; }

	public IFormFile? Avatar { get; set; }
}

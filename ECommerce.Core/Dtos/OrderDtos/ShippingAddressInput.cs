using ECommerce.Core.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.OrderDtos;
public class ShippingAddressInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	[MaxLength(50, ErrorMessage = L.Validation.MaxLength)]
	public string RecipientName { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[Phone(ErrorMessage = L.Validation.Phone)]
	[RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = L.Validation.InvalidPhoneEgypt)]
	public string PhoneNumber { get; set; }

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

using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.OrderDtos;
public class CreateOrderInput
{
	[Required(ErrorMessage = L.Validation.Required)]
	public string CartId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	[Range(1, int.MaxValue, ErrorMessage = L.Validation.Range)]
	public int DeliveryMethodId { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public ShippingAddressInput ShippingAddress { get; set; }

	[Required(ErrorMessage = L.Validation.Required)]
	public PaymentMethod PaymentMethod { get; set; }
}

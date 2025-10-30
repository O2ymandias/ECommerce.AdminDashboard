namespace ECommerce.Core.Dtos.CheckoutDtos;
public class CheckoutResult
{
	public bool Success { get; set; }
	public string Message { get; set; }
	public string RedirectUrl { get; set; }
}

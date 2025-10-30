namespace ECommerce.Core.Dtos.OrderDtos;
public class OrderCancelationResult
{
	public bool ManageToCancel { get; set; }
	public string? CheckoutSessionId { get; set; }

	public string Message { get; set; }
}

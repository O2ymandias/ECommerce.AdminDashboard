namespace ECommerce.Core.Dtos.OrderDtos;
public class CreateOrderResult
{
	public bool Success { get; set; }
	public string Message { get; set; }
	public int CreatedOrderId { get; set; }
}

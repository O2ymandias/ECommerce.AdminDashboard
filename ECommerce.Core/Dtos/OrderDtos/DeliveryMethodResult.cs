namespace ECommerce.Core.Dtos.OrderDtos;
public class DeliveryMethodResult
{
	public int Id { get; set; }
	public string ShortName { get; set; }
	public string Description { get; set; }
	public string DeliveryTime { get; set; }
	public decimal Cost { get; set; }
}

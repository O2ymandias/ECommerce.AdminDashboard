namespace ECommerce.Core.Dtos.OrderDtos;
public class CancelOrderResult
{
	public bool ManageToCancelOrder { get; set; }
	public bool? ManageToExpireSession { get; set; }
	public string? CancelMessage { get; set; }
	public string? ExpireMessage { get; set; }
}

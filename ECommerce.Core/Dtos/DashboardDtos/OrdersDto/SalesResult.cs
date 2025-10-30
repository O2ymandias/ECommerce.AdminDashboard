namespace ECommerce.Core.Dtos.DashboardDtos.OrdersDto;

public class SalesResult
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductPictureUrl { get; set; }
    public int UnitsSold { get; set; }
    public decimal TotalSales { get; set; }
}
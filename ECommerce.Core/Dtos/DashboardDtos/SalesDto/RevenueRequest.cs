namespace ECommerce.Core.Dtos.DashboardDtos.SalesDto;

public class RevenueRequest
{
    public string? UserId { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
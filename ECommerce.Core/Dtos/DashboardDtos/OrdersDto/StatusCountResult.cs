using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.OrdersDto;

public class StatusCountResult<T>
{
    public T Status { get; set; }
    public int Count { get; set; }
}
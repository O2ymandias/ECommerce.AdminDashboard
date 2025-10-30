using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Common;
public class OrderSortOptions
{
	public SortDirection Dir { get; set; } = SortDirection.Desc;
	public OrderSortKey Key { get; set; } = OrderSortKey.CreatedAt;
}

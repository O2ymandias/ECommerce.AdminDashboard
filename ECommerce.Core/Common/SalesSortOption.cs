using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Common;

public class SalesSortOption
{
    public SalesSortKey Key { get; set; } = SalesSortKey.UnitsSold;
    public SortDirection Dir { get; set; } = SortDirection.Desc;
}
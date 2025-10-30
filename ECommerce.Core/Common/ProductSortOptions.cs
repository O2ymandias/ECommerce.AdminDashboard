using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Common;
public class ProductSortOptions
{
	public SortDirection Dir { get; set; } = SortDirection.Asc;
	public ProductSortKey Key { get; set; } = ProductSortKey.Name;
}

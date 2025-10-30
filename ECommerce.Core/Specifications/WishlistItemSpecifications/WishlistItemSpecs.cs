using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Models.WishlistModule;

namespace ECommerce.Core.Specifications.WishlistItemSpecifications;
public class WishlistItemSpecs : BaseSpecification<WishlistItem>
{
	public WishlistItemSpecs(
		WishlistItemSpecsParams specsParams,
		bool enablePagination /* = true */,
		bool enableSorting /* = true */,
		bool enableTracking /* = true */,
		bool enableSplittingQuery /*= false*/
		)
		: base()
	{
		ApplyFiltration(specsParams);
		if (enablePagination) ApplyPagination(specsParams.PageNumber, specsParams.PageSize);
		if (enableSorting) Sort();
		if (enableSplittingQuery) IsSplitQueryEnabled = true;
		if (enableTracking) IsTrackingEnabled = true;
	}
	private void ApplyFiltration(WishlistItemSpecsParams specsParams)
	{
		Criteria = wishlistItem =>
			wishlistItem.UserId.Equals(specsParams.UserId) &&
			(!specsParams.ProductId.HasValue || wishlistItem.ProductId == specsParams.ProductId.Value);
	}

	private void Sort() => SortDesc = i => i.CreatedAt;

}

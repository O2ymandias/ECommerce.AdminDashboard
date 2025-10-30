using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Core.Specifications.RatingSpecifications;
public class RatingSpecs : BaseSpecification<Rating>
{
	public RatingSpecs(
		RatingSpecsParams specsParams,
		bool enablePagination,
		bool enableSorting,
		bool enableTracking,
		bool enableSplittingQuery
		)
		: base()
	{
		ApplyFiltration(specsParams);
		if (enablePagination) ApplyPagination(specsParams.PageNumber, specsParams.PageSize);
		if (enableSorting) Sort();
		if (enableTracking) IsTrackingEnabled = true;
		if (enableSplittingQuery) IsSplitQueryEnabled = true;
	}

	private void ApplyFiltration(RatingSpecsParams specsParams)
	{
		Criteria = rating =>
			(string.IsNullOrWhiteSpace(specsParams.UserId) || rating.UserId.Equals(specsParams.UserId)) &&
			(!specsParams.ProductId.HasValue || rating.ProductId == specsParams.ProductId.Value) &&
			(!specsParams.RatingId.HasValue || rating.Id == specsParams.RatingId.Value);
	}

	private void Sort() => SortDesc = r => r.CreatedAt;

}

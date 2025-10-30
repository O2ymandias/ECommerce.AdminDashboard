using ECommerce.Core.Dtos.RatingDtos;

namespace ECommerce.Core.Common.Pagination;
public class RatingPaginationResult : PaginationResult<RatingResult>
{
	public double Average { get; set; }
}

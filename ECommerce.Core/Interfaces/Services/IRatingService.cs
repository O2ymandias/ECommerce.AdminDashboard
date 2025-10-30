using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.RatingDtos;

namespace ECommerce.Core.Interfaces.Services;
public interface IRatingService
{
	Task<RatingPaginationResult> GetRatingsAsync(RatingSpecsParams specsParams);
	Task<double> GetAverageRatingAsync(int productId);
	Task<bool> AddRatingAsync(RatingInput rating, string userId);
	Task<bool> DeleteRatingAsync(int ratingId);
	Task<LatestRatingsResult> GetLatestRatingsAsync(string userId, int limit);
}

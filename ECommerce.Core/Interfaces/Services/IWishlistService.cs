using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.WishlistDtos;

namespace ECommerce.Core.Interfaces.Services;
public interface IWishlistService
{
	public Task<PaginationResult<WishlistItemResult>> GetUserWishListItemsAsync(WishlistItemSpecsParams specsParams);
	public Task<bool> AddToWishListAsync(WishlistItemInput input, string userId);
	public Task<bool> DeleteFromWishlistAsync(WishlistItemInput input, string userId);
	public Task<IReadOnlyList<int>> GetWishlistProductIdsAsync(string userId);
	public Task<int> GetTotalAsync(string userId);
	public Task<bool> ClearWishlistAsync(string userId);
}

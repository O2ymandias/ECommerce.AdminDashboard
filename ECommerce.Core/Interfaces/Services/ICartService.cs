using ECommerce.Core.Dtos.CartDtos;

namespace ECommerce.Core.Interfaces.Services;
public interface ICartService
{
	Task<CartResult?> GetCartAsync(string cartId);
	Task<CartUpdateResult> AddToCartAsync(CartItemInput item);
	Task<CartUpdateResult> RemoveFromCartAsync(CartItemInput item);
	Task<CartUpdateResult> UpdateQuantityAsync(UpdateQuantityInput item);
	Task<bool> DeleteCartAsync(string id);
}

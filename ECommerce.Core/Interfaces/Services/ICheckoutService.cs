using ECommerce.Core.Dtos.CheckoutDtos;

namespace ECommerce.Core.Interfaces.Services;
public interface ICheckoutService
{
	Task<CheckoutResult> CheckoutAsync(int orderId, string userId);
	public Task<CheckoutResult> RetrieveCheckoutSessionAsync(int orderId, string userId);
	public Task<ExpiredCheckoutSessionResult> ExpireCheckoutSessionAsync(string sessionId);
}

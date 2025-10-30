namespace ECommerce.Core.Interfaces.Services;
public interface ICacheService
{
	Task<TResult?> GetCacheAsync<TResult>(string key);
	Task SetCacheAsync<TValue>(string key, TValue value, TimeSpan? expiry);
	Task<bool> RemoveCacheAsync(string key);
}

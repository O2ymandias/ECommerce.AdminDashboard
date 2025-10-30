namespace ECommerce.Core.Interfaces.Repositories;
public interface IRedisRepository
{
	Task<TResult?> GetAsync<TResult>(string key);
	Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
	Task<bool> DeleteAsync(string key);
}

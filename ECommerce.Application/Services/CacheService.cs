using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Interfaces.Services;

namespace ECommerce.Application.Services;
public class CacheService(IRedisRepository redisRepo) : ICacheService
{
	public async Task<TResult?> GetCacheAsync<TResult>(string key) =>
		await redisRepo.GetAsync<TResult?>(key);

	public async Task<bool> RemoveCacheAsync(string key) =>
		await redisRepo.DeleteAsync(key);

	public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry) =>
		await redisRepo.SetAsync(key, value, expiry);
}

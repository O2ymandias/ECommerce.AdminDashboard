using ECommerce.Core.Interfaces.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Repositories.RedisRepo;

public class RedisRepository(IConnectionMultiplexer connection) : IRedisRepository
{
    private readonly TimeSpan _expiry = TimeSpan.FromDays(14);
    private readonly IDatabase _redis = connection.GetDatabase();

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public async Task<TResult?> GetAsync<TResult>(string key)
    {
        var valueJson = await _redis.StringGetAsync(key);
        return valueJson.HasValue
            ? JsonConvert.DeserializeObject<TResult>(valueJson!, _jsonSerializerSettings)
            : default;
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry)
    {
        return await _redis.StringSetAsync(
            key: key,
            value: JsonConvert.SerializeObject(value, _jsonSerializerSettings),
            expiry: expiry ?? _expiry
        );
    }

    public async Task<bool> DeleteAsync(string key) => await _redis.KeyDeleteAsync(key);
}
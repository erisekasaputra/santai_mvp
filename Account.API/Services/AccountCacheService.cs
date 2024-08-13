
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Account.API.Services;

public class AccountCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    public AccountCacheService(
        IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cacheData = await _distributedCache.GetStringAsync(key);

        if (cacheData is null)
        {
            return default;
        }

        var bytes = Encoding.UTF8.GetBytes(cacheData);

        using var stream = new MemoryStream(bytes);

        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var jsonString = JsonSerializer.Serialize(value);

        var bytes = Encoding.UTF8.GetBytes(jsonString);

        await _distributedCache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
    }
}

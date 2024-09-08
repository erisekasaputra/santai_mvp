using Order.API.Applications.Services.Interfaces;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Order.API.Applications.Services;

public class OrderCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public OrderCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }
     
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {

            var db = _connectionMultiplexer.GetDatabase();

            var cacheData = await db.StringGetAsync(key);

            if (cacheData.IsNullOrEmpty)
            {
                return default;
            }

            var bytes = Encoding.UTF8.GetBytes(cacheData!);

            using var stream = new MemoryStream(bytes);

            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
        catch (RedisException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var script = @"
                if redis.call('EXISTS', KEYS[1]) == 0 then
                    redis.call('SET', KEYS[1], ARGV[2])
                    redis.call('EXPIRE', KEYS[1], ARGV[1])
                    return 1
                else    
                    return 0
                end";

            var jsonString = JsonSerializer.Serialize(value);

            var result = (int)await db.ScriptEvaluateAsync(script, [key], [(int)expiration.TotalSeconds, jsonString]);

            return result == 1;
        }
        catch (RedisException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }


    public async Task<bool> CheckIdempotencyKeyAsync(string key)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        catch (RedisException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> SetIdempotencyKeyAsync(string key, TimeSpan ttl)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var script = @"
                if redis.call('EXISTS', KEYS[1]) == 0 then
                    redis.call('SET', KEYS[1], 'true')
                    redis.call('EXPIRE', KEYS[1], ARGV[1])
                    return 1
                else    
                    return 0
                end";

            var result = (int)await db.ScriptEvaluateAsync(script, [key], [(int)ttl.TotalSeconds]);

            return result == 1;
        }
        catch (RedisException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

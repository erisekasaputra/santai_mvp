using StackExchange.Redis;

namespace Account.API.Infrastructures;

public class IdempotencyService : IIdempotencyService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public IdempotencyService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
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
                    return 1
                else    
                    return 0
                end
            ";

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

using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork; 
using MassTransit;
using Polly;
using StackExchange.Redis;  

namespace Account.API.Applications.Services;

public class MechanicCache : IMechanicCache
{ 
    private readonly IConnectionMultiplexer _connectionMultiplexer; 
    private readonly AsyncPolicy _asyncPolicy;
    public MechanicCache(
        IConnectionMultiplexer connectionMultiplexer)
    { 
        _connectionMultiplexer = connectionMultiplexer;
        _asyncPolicy = Policy.Handle<InvalidOperationException>()
                             .Or<RedisConnectionException>() 
                             .WaitAndRetryAsync(1, retryAttempt =>
                                 TimeSpan.FromSeconds(Math.Pow(1, retryAttempt))  
                             );
    }

    public async Task<bool> UpdateLocationAsync(MechanicAvailabilityCache mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = "mechanics:geo";
            var hashKey = $"mechanics:{mechanic.MechanicId}";

            var existingMechanicLocation = await db.GeoPositionAsync(geoKey, mechanic.MechanicId.ToString());
            if (existingMechanicLocation != null) 
            { 
                await db.GeoAddAsync(geoKey, mechanic.Longitude, mechanic.Latitude, mechanic.MechanicId.ToString());
             
                var hashEntries = new HashEntry[]
                {
                    new (nameof(mechanic.Latitude), mechanic.Latitude),
                    new (nameof(mechanic.Longitude), mechanic.Longitude) 
                };

                await db.HashSetAsync(hashKey, hashEntries);

                return true;
            }

            return false;
        }
        catch (Exception)
        { 
            throw;
        }
    }

    public async Task CreateGeoAsync(MechanicAvailabilityCache mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = "mechanics:geo"; 

            await db.GeoAddAsync(geoKey, mechanic.Longitude, mechanic.Latitude, mechanic.MechanicId.ToString());  
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RemoveGeoAsync(Guid mechanicId)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = "mechanics:geo";

            await db.GeoRemoveAsync(geoKey, mechanicId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RemoveHsetAsync(Guid mechanicId)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase(); 
            var hashKey = $"mechanics:{mechanicId}";  
            await db.KeyDeleteAsync(hashKey);   
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CreateMechanicHsetAsync(MechanicAvailabilityCache mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase(); 
            var hashKey = $"mechanics:{mechanic.MechanicId}"; 
            var hashEntries = new HashEntry[]
            {
                new (nameof(mechanic.OrderId), mechanic.OrderId.ToString()),
                new (nameof(mechanic.Latitude), mechanic.Latitude),
                new (nameof(mechanic.Longitude), mechanic.Longitude),
                new (nameof(mechanic.MechanicId), mechanic.MechanicId.ToString()),
            };

            await db.HashSetAsync(hashKey, hashEntries);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Ping()
    {
        try
        { 
            var db = _connectionMultiplexer.GetDatabase();
             
            var ping = await db.PingAsync(); 

            return true;
        }
        catch (Exception ex)
        {
            throw new RedisException(ex.Message);
        }
    }

    public async Task<Guid> AssignOrderToMechanicAsync(MechanicAvailabilityCache mechanic, Guid orderId)
    {
        var result = await _asyncPolicy.ExecuteAsync(async () =>
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var db = _connectionMultiplexer.GetDatabase();
            var hashKey = $"mechanics:{mechanic.MechanicId}";
              
            string luaScript = @"
            local currentOrderId = redis.call('HGET', KEYS[1], ARGV[1])
            if (currentOrderId == false or currentOrderId == '' or currentOrderId == nil) then
                redis.call('HSET', KEYS[1], ARGV[1], ARGV[2])
                return 1 -- Success
            else
                return 0 -- Failure, OrderId already set
            end";

            var result = (int)await db.ScriptEvaluateAsync(luaScript, [hashKey], [nameof(mechanic.OrderId), orderId.ToString()]);
             
            if (result == 0)
            {
                throw new InvalidOperationException();
            }

            return orderId;
        }); 

        return result;
    }


    public async Task<MechanicAvailabilityCache?> FindAvailableMechanicAsync(
        Guid orderId, double latitude, double longitude, double radius)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var geoKey = "mechanics:geo"; 

        var results = await db.GeoRadiusAsync(geoKey, longitude, latitude, radius, GeoUnit.Kilometers);

        if (results.Length == 0)
        { 
            return null;
        }
         
        foreach (var result in results)
        {
            var mechanicId = Guid.Parse(result.Member.ToString()); 

            var blocked = await IsMechanicBlockedFromOrder(mechanicId, orderId); 
            if (blocked)
            {
                continue;
            }

            var mechanic = await GetMechanicAsync(mechanicId); 
            if (mechanic is not null && (mechanic.OrderId is null || mechanic.OrderId == Guid.Empty))
            { 
                return mechanic;
            }
            else
            {
                await RemoveGeoAsync(mechanicId);
                continue;
            }
        }
         
        return null;
    }

    public async Task<MechanicAvailabilityCache?> GetMechanicAsync(Guid mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{mechanicId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;
         
        var hashDict = hashEntries.ToDictionary(x => x.Name.ToString(), x => x.Value);
         
        if (!hashDict.TryGetValue(nameof(MechanicAvailabilityCache.Latitude), out var latitudeValue) || !double.TryParse(latitudeValue.ToString(), out double latitude))
            return null;  

        if (!hashDict.TryGetValue(nameof(MechanicAvailabilityCache.Longitude), out var longitudeValue) || !double.TryParse(longitudeValue.ToString(), out double longitude))
            return null; 
         
        Guid? orderId = null;
        if (hashDict.TryGetValue(nameof(MechanicAvailabilityCache.OrderId), out var orderIdValue) && !orderIdValue.IsNullOrEmpty)
        { 
            if (Guid.TryParse(orderIdValue.ToString(), out Guid parsedOrderId))
            {
                orderId = parsedOrderId;
            }
        }
         
        var mechanic = new MechanicAvailabilityCache
        {
            MechanicId = mechanicId,
            Latitude = latitude,
            Longitude = longitude,
            OrderId = orderId
        };

        return mechanic;
    }

    public async Task<bool> IsMechanicBlockedFromOrder(Guid mechanicId, Guid orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var setKey = $"mechanics_blacklist_orders:{mechanicId}";
         
        var isBlocked = await db.SetContainsAsync(setKey, orderId.ToString());

        return isBlocked;
    }



    private async Task BlockOrder(Guid mechanicId, Guid orderId)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var setKey = $"mechanics_blacklist_orders:{mechanicId}";
            await db.SetAddAsync(setKey, orderId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UnassignOrderFromMechanicAsync(Guid mechanicId, Guid orderId)
    { 
        try
        {
            var result = await _asyncPolicy.ExecuteAsync(async () =>
            {
                if (mechanicId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(mechanicId));
                }

                var mechanic = await GetMechanicAsync(mechanicId);

                if (mechanic is null)
                {
                    return true;
                }

                if (mechanic.OrderId is null)
                {
                    return true;
                }

                if (orderId != mechanic.OrderId)
                {
                    return false;
                }

                await BlockOrder(mechanicId, orderId); 
                var db = _connectionMultiplexer.GetDatabase();
                var hashKey = $"mechanics:{mechanicId}";

                //string luaScript = @"
                //    local currentOrderId = redis.call('HGET', KEYS[1], ARGV[1])
                //    if (currentOrderId == ARGV[2]) then
                //        redis.call('HDEL', KEYS[1], ARGV[1])
                //        return 1 -- Success, OrderId was unassigned
                //    else
                //        return 0 -- Failure, OrderId didn't match
                //    end";

                //var result = (int)await db.ScriptEvaluateAsync(luaScript,
                //    [hashKey],
                //    ["OrderId", string.Empty]); 


                var hashEntries = new HashEntry[]
                {
                    new (nameof(MechanicAvailabilityCache.OrderId), string.Empty) 
                };
                await db.HashSetAsync(hashKey, hashEntries);

                //if (result == 0)
                //{
                //    return false;
                //}

                return true;
            }); 

            return result;
        } 
        catch (Exception ex) when (ex is InvalidOperationException or RedisException) 
        { 
            return false;
        } 
        catch (Exception)
        {
            return false;
        }
    } 
}

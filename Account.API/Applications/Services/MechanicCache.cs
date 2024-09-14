using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork; 
using MassTransit;
using Polly;
using StackExchange.Redis; 
using System.Data;

namespace Account.API.Applications.Services;

public class MechanicCache : IMechanicCache
{
    public IUnitOfWork _unitOfWork;
    private readonly IConnectionMultiplexer _connectionMultiplexer; 
    private readonly AsyncPolicy _asyncPolicy;
    public MechanicCache(
        IConnectionMultiplexer connectionMultiplexer,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _connectionMultiplexer = connectionMultiplexer;
        _asyncPolicy = Policy.Handle<DBConcurrencyException>()
                             .Or<RedisConnectionException>() 
                             .WaitAndRetryAsync(3, retryAttempt =>
                                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  
                             );
    }

    public async Task UpdateLocationAsync(MechanicAvailabilityCache mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = "mechanics:geo";
            var hashKey = $"mechanics:{mechanic.MechanicId}";
             
            await db.GeoAddAsync(geoKey, mechanic.Longitude, mechanic.Latitude, mechanic.MechanicId.ToString());
             
            var hashEntries = new HashEntry[]
            {
                new (nameof(mechanic.Latitude), mechanic.Latitude),
                new (nameof(mechanic.Longitude), mechanic.Longitude) 
            };

            await db.HashSetAsync(hashKey, hashEntries);
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
                throw new DBConcurrencyException();
            }

            return orderId;
        }); 

        return result;
    }


    public async Task<MechanicAvailabilityCache?> FindAvailableMechanicAsync(
        double latitude, double longitude, double radius)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var geoKey = "mechanics:location:geo"; 

        var results = await db.GeoRadiusAsync(geoKey, longitude, latitude, radius, GeoUnit.Kilometers);

        if (results.Length == 0)
        { 
            return null;
        }
         
        foreach (var result in results)
        {
            var mechanicId = Guid.Parse(result.Member.ToString());
            var mechanic = await GetMechanicAsync(mechanicId);

            if (mechanic is not null && ( mechanic.OrderId is null || mechanic.OrderId == Guid.Empty))
            { 
                return mechanic;
            }
            else
            {
                var mechanicFromSql = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(mechanicId); 
                if (mechanicFromSql is not null)
                {
                    var newMechanicCache = new MechanicAvailabilityCache()
                    {
                        MechanicId = mechanicId,
                        Latitude = latitude,
                        Longitude = longitude,
                        OrderId = mechanicFromSql?.MechanicOrderTask?.OrderId
                    };

                    await CreateMechanicHsetAsync(newMechanicCache); 
                    return newMechanicCache;
                }
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

    public async Task<bool> UnassignOrderFromMechanicAsync(Guid mechanicId)
    { 
        var result = await _asyncPolicy.ExecuteAsync(async () =>
        {
            if (mechanicId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(mechanicId));
            }

            var db = _connectionMultiplexer.GetDatabase();
            var hashKey = $"mechanics:{mechanicId}";

            string luaScript = @"
                local currentOrderId = redis.call('HGET', KEYS[1], ARGV[1])
                if (currentOrderId == ARGV[2]) then
                    redis.call('HDEL', KEYS[1], ARGV[1])
                    return 1 -- Success, OrderId was unassigned
                else
                    return 0 -- Failure, OrderId didn't match
                end";

            var result = (int)await db.ScriptEvaluateAsync(luaScript,
                [hashKey],
                ["OrderId", string.Empty]); 

            if (result == 0)
            {
                throw new DBConcurrencyException();
            }

            return true;
        });

        return result;
    } 
}

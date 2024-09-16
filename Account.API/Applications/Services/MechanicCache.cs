using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces; 
using Core.Configurations; 
using Microsoft.Extensions.Options;
using Polly;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Net;

namespace Account.API.Applications.Services;

public class MechanicCache : IMechanicCache
{ 
    private readonly IConnectionMultiplexer _connectionMultiplexer; 
    private readonly AsyncPolicy _asyncPolicy;
    private readonly ILogger<MechanicCache> _logger;
    private RedLockFactory _redLockFactory;
    
    public MechanicCache(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<MechanicCache> logger,
        IOptionsMonitor<CacheConfiguration> cacheOptions)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _asyncPolicy = Policy.Handle<InvalidOperationException>()
                             .Or<RedisConnectionException>() 
                             .WaitAndRetryAsync(1, retryAttempt =>
                                 TimeSpan.FromSeconds(Math.Pow(1, retryAttempt))  
                             );

        

        _redLockFactory = RedLockFactory.Create(ParseRedisEndpoints([cacheOptions.CurrentValue.Host]));
    }
    private async Task<(bool isAcquired, string? resourceLock)> AcquireLockAsync(string mechanicId)
    {
        var lockResource = $"lock:mechanic:{mechanicId}";
        var mechanicLock = await _redLockFactory.CreateLockAsync(lockResource, TimeSpan.FromMinutes(10));

        if (mechanicLock.IsAcquired)
        {
            return (true, lockResource);
        }

        return (false, null);
    }

    private async Task DeleteKeyAsync(string lockResource)
    {
        var db = _connectionMultiplexer.GetDatabase();
         
        await db.KeyDeleteAsync("redlock:"+lockResource);
    }


    private static RedLockEndPoint[] ParseRedisEndpoints(string[] redisEndpoints)
    { 
        var redLockEndpoints = new RedLockEndPoint[redisEndpoints.Length];
        for (int i = 0; i < redisEndpoints.Length; i++)
        {
            var parts = redisEndpoints[i].Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out var port))
            {
                redLockEndpoints[i] = new RedLockEndPoint
                {
                    EndPoint = new DnsEndPoint(parts[0], port)
                };
            }
            else
            {
                throw new ArgumentException($"Endpoint {redisEndpoints[i]} is not valid. Format should be 'host:port'");
            }
        }
        return redLockEndpoints;
    } 

    public async Task<bool> UpdateLocationAsync(MechanicExistence mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = "mechanics:geo";
            var hashKey = $"mechanics:{mechanic.MechanicId}";

            var existingMechanicLocation = await db.GeoPositionAsync(geoKey, mechanic.MechanicId.ToString());
            if (existingMechanicLocation is not null) 
            {
                await CreateGeoAsync(mechanic);
                 
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

    private double RedisValueToDouble(RedisValue value)
    {
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return 0;
        }

        return double.TryParse(value.ToString(), out var result) ? result : 0;  
    }

    private DateTime RedisValueToDateTime(RedisValue value)
    {
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return default;
        }

        return DateTime.TryParse(value.ToString(), out var result) ? result : default;
    } 

    public string RedisValueToString(RedisValue value)
    {
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return string.Empty;
        }

        return value.ToString();
    }



    public async Task<MechanicExistence?> GetMechanicHashSetAsync(string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{mechanicId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;


        var latitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Latitude)).Value);
        var longitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Longitude)).Value);
        var orderId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.OrderId)).Value);
        var status = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Status)).Value);



        return new MechanicExistence(mechanicId.ToString(), orderId, latitude, longitude, status);
    }


    public async Task<OrderTask?> GetOrderTaskAsync(string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"orders:waiting_mechanic_assign:data:{orderId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;

        var buyerId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTask.BuyerId)).Value);
        var mechanicId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTask.MechanicId)).Value);
        var latitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTask.Latitude)).Value);
        var longitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTask.Longitude)).Value);
        var status = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTask.OrderStatus)).Value);

        return new OrderTask(buyerId, orderId.ToString(), mechanicId, latitude, longitude, status);
    }

    public async Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{orderId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;
         
        var mechanicId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTaskMechanicConfirm.MechanicId)).Value);

        var expireAtUtc = RedisValueToDateTime(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTaskMechanicConfirm.ExpiredAtUtc)).Value);

        return new OrderTaskMechanicConfirm(orderId.ToString(), mechanicId, expireAtUtc);
    }


    public async Task CreateGeoAsync(MechanicExistence mechanic)
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

    public async Task CreateMechanicHashSetAsync(MechanicExistence mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase(); 
            var hashKey = $"mechanics:{mechanic.MechanicId}"; 
            var hashEntries = new HashEntry[]
            {
                new (nameof(mechanic.OrderId), mechanic.OrderId),
                new (nameof(mechanic.Latitude), mechanic.Latitude),
                new (nameof(mechanic.Longitude), mechanic.Longitude),
                new (nameof(mechanic.MechanicId), mechanic.MechanicId),
                new (nameof(mechanic.Status), mechanic.Status)
            };

            await db.HashSetAsync(hashKey, hashEntries);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CreateOrderHashSetAsync(OrderTask order)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var hashKey = $"orders:waiting_mechanic_assign:data:{order.OrderId}";
            var hashEntries = new HashEntry[]
            {
                new (nameof(order.BuyerId), order.BuyerId),
                new (nameof(order.OrderId), order.OrderId),
                new (nameof(order.MechanicId), order.MechanicId), 
                new (nameof(order.Latitude), order.Latitude),
                new (nameof(order.Longitude), order.Longitude),
                new (nameof(order.OrderStatus), order.OrderStatus)
            };

            await db.HashSetAsync(hashKey, hashEntries);
        }
        catch (Exception)
        {
            throw;
        }
    } 

    public async Task<bool> Activate(string mechanicId)
    {
        var mechanic = await GetMechanicHashSetAsync(mechanicId);

        if (mechanic is null)
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Available);  
            await CreateMechanicHashSetAsync(mech); 
            await CreateGeoAsync(mech);

            return true;
        }
        else
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Available);   
            if (mechanic.Status == MechanicStatus.Unavailable)
            { 
                await CreateMechanicHashSetAsync(mech); 
                return true;
            }
            await CreateGeoAsync(mech);

            return true;
        }
    }

    public async Task<bool> Deactivate(string mechanicId)
    {
        var mechanic = await GetMechanicHashSetAsync(mechanicId);

        if (mechanic is null)
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Unavailable);
            await CreateMechanicHashSetAsync(mech);
            return true;
        }
        else
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Unavailable);
            if (mechanic.Status == MechanicStatus.Available)
            {
                await CreateMechanicHashSetAsync(mech); 
                return true;
            }

            return true;
        }
    }

    public async Task<bool> PingAsync()
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
     
     
    public async Task CreateOrderToQueueAndHash(OrderTask orderTask)
    {  
        try
        {
            var db = _connectionMultiplexer.GetDatabase();  
            await db.ListRemoveAsync("orders:waiting_mechanic_assign:queue", orderTask.OrderId);
            await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderTask.OrderId);
            await CreateOrderHashSetAsync(orderTask);
        }
        catch (Exception)
        {
            throw;
        }   
    }

    private async Task AddOrderFIFOtoQueue(IDatabase db, string orderId)
    {
        try
        {
            await db.ListRemoveAsync("orders:waiting_mechanic_assign:queue", orderId); 
            await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId); 
        }
        catch (Exception)
        {
            throw;
        }
    } 
    public async Task<bool> AcceptOrderByMechanic(string orderId, string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var confirmation = await GetOrderWaitingMechanicConfirmAsync(orderId);
            if (confirmation is null)
            {
                return false;
            }

            if (confirmation.ExpiredAtUtc > DateTime.UtcNow) // not expired already
            {
                var resource = $"orders:waiting_mechanic_confirm:data:{confirmation.OrderId}";
                await db.KeyDeleteAsync(resource);

                var order = await GetOrderTaskAsync(orderId);
                if (order is null)
                {
                    return false;
                }

                if (order.MechanicId != mechanicId.ToString())
                {
                    return false;
                }

                if (string.IsNullOrEmpty(order.MechanicId))
                {
                    return false;   
                }

                var mechanic = await GetMechanicHashSetAsync(order.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    mechanic.SetOrder(orderId.ToString());
                    mechanic.SetMechanicStatus(MechanicStatus.Bussy);
                    await CreateMechanicHashSetAsync(mechanic);
                }

                order.SetOrderStatus(OrderTaskStatus.MechanicAssigned);
                order.SetMechanic(mechanicId.ToString());
                await CreateOrderHashSetAsync(order);  

                return true;
            }
            else
            {  
                var resource = $"orders:waiting_mechanic_confirm:data:{confirmation.OrderId}";
                await db.KeyDeleteAsync(resource);



                var order = await GetOrderTaskAsync(confirmation.OrderId);
                if (order is null)
                {
                    return false;
                }

                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(order);




                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(confirmation.MechanicId, orderId);
                }

                await db.ListRemoveAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);
                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);

                return false;
            }
        }

        return false;
    }

    public async Task<bool> RejectOrderByMechanic(string mechanicId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var confirmation = await GetOrderWaitingMechanicConfirmAsync(orderId);
            if (confirmation is null)
            {
                return false;
            }

            if (confirmation.ExpiredAtUtc > DateTime.UtcNow) // not expired already
            { 

                var order = await GetOrderTaskAsync(orderId);
                if (order is null)
                {
                    return false;
                }

                if (order.MechanicId != mechanicId.ToString())
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(order.MechanicId))
                {
                    var mechanic = await GetMechanicHashSetAsync(order.MechanicId);

                    if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                    {
                        mechanic.ResetOrder();
                        mechanic.SetMechanicStatus(MechanicStatus.Available);
                        await CreateMechanicHashSetAsync(mechanic);
                        await BlockMechanicToAnOrder(mechanic.MechanicId, orderId);
                    }
                }


                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(order);



                await AddOrderFIFOtoQueue(db, orderId);

                return true; 
            }
            else  
            {
                var resource = $"orders:waiting_mechanic_confirm:data:{confirmation.OrderId}";
                await db.KeyDeleteAsync(resource);



                var order = await GetOrderTaskAsync(confirmation.OrderId);
                if (order is null)
                {
                    return false;
                }

                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(order);




                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    mechanic.ResetOrder(); ;
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(confirmation.MechanicId, orderId);
                }
                 
                await db.ListRemoveAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);
                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);

                return false;
            }  
        }

        return false;
    }

    public async Task<bool> CancelOrderByMechanic(string mechanicId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var order = await GetOrderTaskAsync(orderId);
            if (order is null)
            {
                return false;
            }

            if (order.MechanicId != mechanicId.ToString())
            {
                return false;
            }

            if (!string.IsNullOrEmpty(order.MechanicId))
            {
                var mechanic = await GetMechanicHashSetAsync(order.MechanicId);

                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    mechanic.ResetOrder();  
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(mechanic.MechanicId, orderId);
                }
            }


            order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
            order.ResetMechanic();
            await CreateOrderHashSetAsync(order);  

            await AddOrderFIFOtoQueue(db, orderId); 

            return true;
        }

        return false;
    }


    public async Task<bool> CancelOrderByUser(string buyerId, string orderId)
    {  
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {   

            var order = await GetOrderTaskAsync(orderId);
            if (order is null)
            {
                return false;
            }

            if (order.BuyerId != buyerId.ToString()) 
            {
                return false;
            }


            if (!string.IsNullOrEmpty(order.MechanicId))
            {
                var mechanic = await GetMechanicHashSetAsync(order.MechanicId);

                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(mechanic); 
                    await DeleteKeyAsync($"lock:mechanic:{mechanic.MechanicId}");
                } 
            }


            order.SetOrderStatus(OrderTaskStatus.OrderTaskCompleted);
            order.ResetMechanic();
            await CreateOrderHashSetAsync(order);









            return true;
        }

        return false;
    }

    public async Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();
        var orderId = await db.ListLeftPopAsync("orders:waiting_mechanic_confirm:queue");

        if (string.IsNullOrEmpty(orderId) || !orderId.HasValue || orderId.IsNullOrEmpty)
        {
            return;
        }

        await TryProcessExpiryMechanicConfirmationAsync(db, orderId.ToString());
    }




    private async Task TryProcessExpiryMechanicConfirmationAsync(IDatabase db, string orderId)
    {
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10)); 
        if (lockAcquired.IsAcquired)
        {
            var confirmation = await GetOrderWaitingMechanicConfirmAsync(orderId);
            if (confirmation is null)
            {
                return;
            }

            if (confirmation.ExpiredAtUtc > DateTime.UtcNow) // not expired already
            {
                await db.ListRemoveAsync("orders:waiting_mechanic_confirm:queue", confirmation.OrderId);
                await db.ListRightPushAsync("orders:waiting_mechanic_confirm:queue", confirmation.OrderId);
            }
            else // has expired, process it 
            {
                var resource = $"orders:waiting_mechanic_confirm:data:{confirmation.OrderId}";
                await db.KeyDeleteAsync(resource);

               

                var order = await GetOrderTaskAsync(confirmation.OrderId);
                if (order is null)
                {
                    return;
                }
                 

                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(order);
                 


                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == confirmation.OrderId)
                {
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(mechanic);
                }


                await BlockMechanicToAnOrder(confirmation.MechanicId, orderId); 

                await db.ListRemoveAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);
                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId);
            }

            return;
        }

        await db.ListRightPushAsync("orders:waiting_mechanic_confirm:queue", orderId.ToString());
    }














    public async Task ProcessOrdersWaitingMechanicAssignFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();

        var orderId = await db.ListLeftPopAsync("orders:waiting_mechanic_assign:queue");
        if (orderId == string.Empty || string.IsNullOrEmpty(orderId) || orderId.IsNullOrEmpty)
        {
            return;
        }

        await TryAssignMechanicToOrderAsync(db, orderId.ToString());
    }

    private async Task TryAssignMechanicToOrderAsync(IDatabase db, string orderId)
    {
        try
        {
            var key = $"lock:order:{orderId}";
            using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(30));

            if (!lockAcquired.IsAcquired)
            {
                await db.ListRemoveAsync("orders:waiting_mechanic_assign:queue", orderId);
                await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId);
                return;
            }

            var order = await GetOrderTaskAsync(orderId);

            if (order is null) return;

            if (order.OrderStatus == OrderTaskStatus.OrderTaskCompleted || order.OrderStatus == OrderTaskStatus.MechanicAssigned)
            {
                return;
            }

            var mechanics = await db.GeoRadiusAsync(
                "mechanics:geo", order.Longitude, order.Latitude, 30, GeoUnit.Miles, count: 100, order: Order.Ascending);

            foreach (var mechanic in mechanics)
            {
                if (!mechanic.Member.HasValue || mechanic.Member.IsNullOrEmpty)
                {
                    continue;
                }

                var mechanicId = mechanic.Member.ToString();
                (var isAcquired, var lockResource) = await AcquireLockAsync(mechanicId);

                if (isAcquired)
                {
                    var blocked = await IsMechanicBlockedFromOrder(mechanicId.ToString(), orderId);
                    if (blocked)
                    {
                        await DeleteKeyAsync(lockResource!);
                        continue;
                    }

                    var mechanicData = await GetMechanicHashSetAsync(mechanicId.ToString());
                    if (mechanicData is null)
                    {
                        await DeleteKeyAsync(lockResource!);
                        continue;
                    }

                    if (mechanicData.Status is MechanicStatus.Available)
                    {
                        order.SetOrderStatus(OrderTaskStatus.MechanicAssigned);
                        order.SetMechanic(mechanicId.ToString());
                        await CreateOrderHashSetAsync(order);


                        mechanicData.SetOrder(orderId.ToString());
                        mechanicData.SetMechanicStatus(MechanicStatus.Bussy);
                        await CreateMechanicHashSetAsync(mechanicData);


                        await OrderWaitingConfirmMechanic(db, order, mechanicData);
                        return;
                    }

                    if (mechanicData.Status is MechanicStatus.Unavailable)
                    {
                        await DeleteKeyAsync(lockResource!);
                    }
                }
            }
             
            await db.ListRemoveAsync("orders:waiting_mechanic_assign:queue", orderId);
            await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId);
        }
        catch (Exception)
        {
            await db.ListRemoveAsync("orders:waiting_mechanic_assign:queue", orderId);
            await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId);
        }
    }
     
    public async Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic)
    { 
        await db.ListRemoveAsync("orders:waiting_mechanic_confirm:queue", order.OrderId.ToString());
        await db.ListRightPushAsync("orders:waiting_mechanic_confirm:queue", order.OrderId.ToString());

        var resource = $"orders:waiting_mechanic_confirm:data:{order.OrderId}";  
        var hashEntries = new HashEntry[]
        {
            new (nameof(OrderTaskMechanicConfirm.OrderId), order.OrderId.ToString()),
            new (nameof(OrderTaskMechanicConfirm.MechanicId), mechanic.MechanicId.ToString()),
            new (nameof(OrderTaskMechanicConfirm.ExpiredAtUtc), DateTime.UtcNow.AddSeconds(60).ToString())
        };

        await db.HashSetAsync(resource, hashEntries); 
    }

    public async Task<bool> IsMechanicBlockedFromOrder(string mechanicId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var setKey = $"mechanics:order:blacklist:{mechanicId}"; 
        var isBlocked = await db.SetContainsAsync(setKey, orderId.ToString());

        return isBlocked;
    } 

    private async Task BlockMechanicToAnOrder(string mechanicId, string orderId)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var setKey = $"mechanics:order:blacklist:{mechanicId}";
            await db.SetAddAsync(setKey, orderId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    } 
}

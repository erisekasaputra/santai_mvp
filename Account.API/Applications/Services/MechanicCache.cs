using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces; 
using Core.Configurations;
using MassTransit;
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
    private async Task<(bool isAcquired, string? resourceLock)> AcquireLockAsync(Guid mechanicId)
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
        await db.KeyDeleteAsync(lockResource);
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

    public async Task<MechanicExistence?> GetMechanicHashSetAsync(Guid mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{mechanicId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;

        var hashDict = hashEntries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

        if (!double.TryParse(hashDict.GetValueOrDefault(nameof(MechanicExistence.Latitude), string.Empty), out double latitude) ||
            !double.TryParse(hashDict.GetValueOrDefault(nameof(MechanicExistence.Longitude), string.Empty), out double longitude))
        {
            return null;
        }

        Guid? orderId = null;
        if (Guid.TryParse(hashDict.GetValueOrDefault(nameof(MechanicExistence.OrderId), string.Empty), out Guid parsedOrderId))
        {
            orderId = parsedOrderId;
        }

        var status = hashDict.GetValueOrDefault(nameof(MechanicExistence.MechanicStatus), MechanicStatus.Unavailable);

        return new MechanicExistence
        {
            MechanicId = mechanicId,
            Latitude = latitude,
            Longitude = longitude,
            OrderId = orderId,
            MechanicStatus = status
        };
    }


    public async Task<OrderTask?> GetOrderTaskAsync(Guid orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{orderId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;

        var hashDict = hashEntries.ToDictionary(
            x => x.Name.ToString(), x => x.Value.ToString());

        if (!double.TryParse(
            hashDict.GetValueOrDefault(nameof(OrderTask.Latitude), string.Empty), out double latitude) 
            ||
            !double.TryParse(
            hashDict.GetValueOrDefault(nameof(OrderTask.Longitude), string.Empty), out double longitude))
        {
            return null;
        }

        Guid? mechanicId = null;
        if (Guid.TryParse(
            hashDict.GetValueOrDefault(nameof(OrderTask.MechanicId), string.Empty), out Guid parsedMechanicId))
        {
            mechanicId = parsedMechanicId;
        }

        Guid buyerId = Guid.Parse(hashDict.GetValueOrDefault(nameof(OrderTask.BuyerId), string.Empty));

        var status = hashDict.GetValueOrDefault(nameof(OrderTask.OrderStatus), OrderTaskStatus.WaitingMechanic);

        return new OrderTask
        {
            BuyerId = buyerId,
            OrderId = orderId,
            MechanicId = mechanicId,
            Latitude = latitude,
            Longitude = longitude,
            OrderStatus = status
        };
    }

    public async Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(Guid orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = $"mechanics:{orderId}";
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;

        var hashDict = hashEntries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

        if (!DateTime.TryParse(
            hashDict.GetValueOrDefault(nameof(OrderTaskMechanicConfirm.ExpiredAtUtc), string.Empty), out DateTime expireAtUtc))
        {
            return null;
        }
         
        bool success = Guid.TryParse(
            hashDict.GetValueOrDefault(nameof(OrderTaskMechanicConfirm.MechanicId), string.Empty), out Guid mechanicId);

        if (!success)
        {
            return null;
        }

        return new OrderTaskMechanicConfirm
        {
            OrderId = orderId,
            MechanicId = mechanicId,
            ExpiredAtUtc = expireAtUtc
        };
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
                new (nameof(mechanic.OrderId), mechanic.OrderId.ToString()),
                new (nameof(mechanic.Latitude), mechanic.Latitude),
                new (nameof(mechanic.Longitude), mechanic.Longitude),
                new (nameof(mechanic.MechanicId), mechanic.MechanicId.ToString()),
                new (nameof(mechanic.MechanicStatus), mechanic.MechanicStatus)
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
                new (nameof(order.OrderId), order.OrderId.ToString()),
                new (nameof(order.MechanicId), (order.MechanicId is null ? string.Empty : order.MechanicId.Value.ToString())), 
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

    public async Task<bool> Activate(Guid mechanicId)
    {
        var mechanic = await GetMechanicHashSetAsync(mechanicId);

        if (mechanic is null)
        {
            await CreateMechanicHashSetAsync(new MechanicExistence()
            {
                MechanicId = mechanicId,
                Latitude = 0,
                Longitude = 0,
                MechanicStatus = MechanicStatus.Available
            });
            return true;
        }
        else
        {
            if (mechanic.MechanicStatus == MechanicStatus.Unavailable)
            {
                await CreateMechanicHashSetAsync(new MechanicExistence()
                {
                    MechanicId = mechanicId,
                    Latitude = 0,
                    Longitude = 0,
                    MechanicStatus = MechanicStatus.Available
                });

                return true;
            }

            return false;
        }
    }

    public async Task<bool> Deactivate(Guid mechanicId)
    {
        var mechanic = await GetMechanicHashSetAsync(mechanicId);

        if (mechanic is null)
        {
            await CreateMechanicHashSetAsync(new MechanicExistence()
            {
                MechanicId = mechanicId,
                Latitude = 0,
                Longitude = 0,
                MechanicStatus = MechanicStatus.Unavailable
            });
            return true;
        }
        else
        {
            if (mechanic.MechanicStatus == MechanicStatus.Available)
            {
                await CreateMechanicHashSetAsync(new MechanicExistence()
                {
                    MechanicId = mechanicId,
                    Latitude = 0,
                    Longitude = 0,
                    MechanicStatus = MechanicStatus.Unavailable
                });

                return true;
            }

            return false;
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
            var resourceOrderQueue = "orders:waiting_mechanic_assign:queue";
            await db.ListRightPushAsync(resourceOrderQueue, orderTask.OrderId.ToString());
            await CreateOrderHashSetAsync(orderTask);
        }
        catch (Exception)
        {
            throw;
        }   
    }

    private async Task AddOrderFIFOtoQueue(IDatabase db, Guid orderId)
    {
        try
        { 
            var resourceOrderQueue = "orders:waiting_mechanic_assign:queue";
            await db.ListRightPushAsync(resourceOrderQueue, orderId.ToString()); 
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<Guid?> PopOrderFIFOfromQueue(IDatabase db)
    {
        try
        {
            var resourceOrderQueue = "orders:waiting_mechanic_assign:queue";
            var orderId = await db.ListLeftPopAsync(resourceOrderQueue);

            if (orderId.IsNullOrEmpty)
            {
                return null;
            }

            return Guid.Parse(orderId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> AcceptOrderByMechanic(Guid orderId, Guid mechanicId)
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

                if (order.MechanicId != mechanicId)
                {
                    return false;
                }

                if (order.MechanicId is null)
                {
                    return false;   
                }

                var mechanic = await GetMechanicHashSetAsync(order.MechanicId.Value);
                if (mechanic is not null && mechanic.OrderId == orderId)
                {
                    mechanic.OrderId = orderId;
                    mechanic.MechanicStatus = MechanicStatus.Bussy;
                    await CreateMechanicHashSetAsync(mechanic);
                }

                order.OrderStatus = OrderTaskStatus.MechanicAssigned;
                order.MechanicId = mechanicId;
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

                order.OrderStatus = OrderTaskStatus.WaitingMechanic;
                order.MechanicId = null;
                await CreateOrderHashSetAsync(order);




                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId)
                {
                    mechanic.OrderId = null;
                    mechanic.MechanicStatus = MechanicStatus.Available;
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(confirmation.MechanicId, orderId);
                }

                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId.ToString());

                return false;
            }
        }

        return false;
    }

    public async Task<bool> RejectOrderByMechanic(Guid mechanicId, Guid orderId)
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

                if (order.MechanicId != mechanicId)
                {
                    return false;
                }

                if (order.MechanicId is not null)
                {
                    var mechanic = await GetMechanicHashSetAsync(order.MechanicId.Value);

                    if (mechanic is not null && mechanic.OrderId == orderId)
                    {
                        mechanic.OrderId = null;
                        mechanic.MechanicStatus = MechanicStatus.Available;
                        await CreateMechanicHashSetAsync(mechanic);
                        await BlockMechanicToAnOrder(mechanic.MechanicId, orderId);
                    }
                }


                order.OrderStatus = OrderTaskStatus.WaitingMechanic;
                order.MechanicId = null;
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

                order.OrderStatus = OrderTaskStatus.WaitingMechanic;
                order.MechanicId = null;
                await CreateOrderHashSetAsync(order);




                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId)
                { 
                    mechanic.OrderId = null;
                    mechanic.MechanicStatus = MechanicStatus.Available;
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(confirmation.MechanicId, orderId);
                }
                 
                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId.ToString());

                return false;
            }  
        }

        return false;
    }

    public async Task<bool> CancelOrderByMechanic(Guid mechanicId, Guid orderId)
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

            if (order.MechanicId != mechanicId)
            {
                return false;
            }

            if (order.MechanicId is not null)
            {
                var mechanic = await GetMechanicHashSetAsync(order.MechanicId.Value);

                if (mechanic is not null && mechanic.OrderId == orderId)
                {
                    mechanic.OrderId = null;
                    mechanic.MechanicStatus = MechanicStatus.Available;
                    await CreateMechanicHashSetAsync(mechanic);
                    await BlockMechanicToAnOrder(mechanic.MechanicId, orderId);
                }
            }


            order.OrderStatus = OrderTaskStatus.WaitingMechanic;
            order.MechanicId = null;
            await CreateOrderHashSetAsync(order);



            await AddOrderFIFOtoQueue(db, orderId); 

            return true;
        }

        return false;
    }


    public async Task<bool> CancelOrderByUser(Guid buyerId, Guid orderId)
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

            if (order.BuyerId != buyerId) 
            {
                return false;
            }


            if (order.MechanicId is not null)
            {
                var mechanic = await GetMechanicHashSetAsync(order.MechanicId.Value);

                if (mechanic is not null && mechanic.OrderId == orderId)
                {
                    mechanic.OrderId = null;
                    mechanic.MechanicStatus = MechanicStatus.Available;
                    await CreateMechanicHashSetAsync(mechanic); 
                    await DeleteKeyAsync($"lock:mechanic:{mechanic.MechanicId}");
                } 
            }


            order.OrderStatus = OrderTaskStatus.OrderTaskCompleted;
            order.MechanicId = null;
            await CreateOrderHashSetAsync(order);









            return true;
        }

        return false;
    }

    public async Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();
        var orderId = await db.ListLeftPopAsync("orders:waiting_mechanic_confirm:queue");

        if (string.IsNullOrEmpty(orderId) || !orderId.HasValue)
        {
            return;
        }

        await TryProcessExpiryMechanicConfirmationAsync(db, Guid.Parse(orderId.ToString()));
    }

    private async Task TryProcessExpiryMechanicConfirmationAsync(IDatabase db, Guid orderId)
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
                await db.ListRightPushAsync("orders:waiting_mechanic_confirm:queue", confirmation.OrderId.ToString());
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
                 

                order.OrderStatus = OrderTaskStatus.WaitingMechanic;
                order.MechanicId = null;
                await CreateOrderHashSetAsync(order);
                 


                var mechanic = await GetMechanicHashSetAsync(confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == confirmation.OrderId)
                { 
                    mechanic.OrderId = null;
                    mechanic.MechanicStatus = MechanicStatus.Available;
                    await CreateMechanicHashSetAsync(mechanic);
                }


                await BlockMechanicToAnOrder(confirmation.MechanicId, orderId); 
                await db.ListRightPushAsync("order:waiting_mechanic_assign:queue", confirmation.OrderId.ToString());
            }

            return;
        }

        await db.ListRightPushAsync("orders:waiting_mechanic_confirm:queue", orderId.ToString());
    }














    public async Task ProcessOrdersWaitingMechanicAssignFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();

        var orderId = await PopOrderFIFOfromQueue(db);
        if (orderId is null || orderId == Guid.Empty)
        {
            return;
        }

        await TryAssignMechanicToOrderAsync(db, orderId.Value);
    }

    private async Task TryAssignMechanicToOrderAsync(IDatabase db, Guid orderId)
    {
        var key = $"lock:order:{orderId}";
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(30));

        if (!lockAcquired.IsAcquired)
        {
            await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId.ToString());
            return;
        }

        var order = await GetOrderTaskAsync(orderId);

        if (order is null) return; 

        if (order.OrderStatus == OrderTaskStatus.OrderTaskCompleted || order.OrderStatus == OrderTaskStatus.MechanicAssigned)
        {
            return;
        }
          
        var mechanics = await db.GeoRadiusAsync("mechanics:geo", order.Latitude, order.Longitude, 20, GeoUnit.Kilometers, count: 100, order: Order.Ascending);

        foreach (var mechanic in mechanics)
        {
            var mechanicId = Guid.Parse(mechanic.Member.ToString());
            (var isAcquired, var lockResource) = await AcquireLockAsync(mechanicId);

            try
            { 
                if (isAcquired)
                { 
                    var blocked = await IsMechanicBlockedFromOrder(Guid.Parse(mechanicId.ToString()), orderId);
                    if (blocked)
                    {
                        await DeleteKeyAsync(lockResource!);
                        continue;
                    }

                    var mechanicData = await GetMechanicHashSetAsync(Guid.Parse(mechanicId.ToString()));
                    if (mechanicData is null)
                    {
                        await DeleteKeyAsync(lockResource!);
                        continue;
                    }

                    if (mechanicData.MechanicStatus == MechanicStatus.Available)
                    {
                        order.OrderStatus = OrderTaskStatus.MechanicAssigned;
                        order.MechanicId = mechanicId;
                        await CreateOrderHashSetAsync(order);


                        mechanicData.OrderId = orderId;
                        mechanicData.MechanicStatus = MechanicStatus.Bussy;
                        await CreateMechanicHashSetAsync(mechanicData);


                        await OrderWaitingConfirmMechanic(db, order, mechanicData);
                        return;
                    }
                }
            }
            catch (Exception) 
            {
                await DeleteKeyAsync(lockResource!);
                continue;
            }
        }
         
        await db.ListRightPushAsync("orders:waiting_mechanic_assign:queue", orderId.ToString());
    }
     
    public async Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic)
    { 
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

    public async Task<bool> IsMechanicBlockedFromOrder(Guid mechanicId, Guid orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var setKey = $"mechanics:order:blacklist:{mechanicId}";
         
        var isBlocked = await db.SetContainsAsync(setKey, orderId.ToString());

        return isBlocked;
    } 

    private async Task BlockMechanicToAnOrder(Guid mechanicId, Guid orderId)
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

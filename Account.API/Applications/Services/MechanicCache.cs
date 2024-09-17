using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Account.API.SeedWork;
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
    public async Task<bool> UpdateLocationAsync(MechanicExistence mechanic)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var geoKey = CacheKey.MechanicGeo();
            var hashKey = CacheKey.MechanicExistencePrefix(mechanic.MechanicId);

            var existingMechanicLocation = await db.GeoPositionAsync(geoKey, mechanic.MechanicId.ToString());
            if (existingMechanicLocation is not null) 
            {
                await CreateGeoAsync(db, mechanic);
                 
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

    public string RedisValueToString(RedisValue value)
    {
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return string.Empty;
        }

        return value.ToString();
    } 
  

    public async Task<bool> Activate(string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase(); 
        var mechanic = await GetMechanicHashSetAsync(db, mechanicId);

        if (mechanic is null)
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Available);  
            await CreateMechanicHashSetAsync(db, mech); 
            await CreateGeoAsync(db, mech);

            return true;
        }
        else
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Available);   
            if (mechanic.Status == MechanicStatus.Unavailable)
            { 
                await CreateMechanicHashSetAsync(db, mech); 
                return true;
            }
            await CreateGeoAsync(db, mech);

            return true;
        }
    }

    public async Task<bool> Deactivate(string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var mechanic = await GetMechanicHashSetAsync(db, mechanicId); 
        if (mechanic is null)
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Unavailable);
            await CreateMechanicHashSetAsync(db, mech);
            return true;
        }
        else
        {
            var mech = new MechanicExistence(mechanicId.ToString(), string.Empty, 0, 0, MechanicStatus.Unavailable);
            if (mechanic.Status == MechanicStatus.Available)
            {
                await CreateMechanicHashSetAsync(db, mech); 
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
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderTask.OrderId);
            await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderTask.OrderId);
            await CreateOrderHashSetAsync(db, orderTask);
        }
        catch (Exception)
        {
            throw;
        }   
    } 
   

     
    public async Task<bool> AcceptOrderByMechanic(string orderId, string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var confirmation = await GetOrderWaitingMechanicConfirmAsync(db, orderId);
            if (confirmation is null)
            {
                return false;
            }

            if (confirmation.ExpiredAtUtc <= DateTime.UtcNow) // not expired already
            {
                return false;
            } 


            var order = await GetOrderTaskAsync(db, orderId);
            if (order is null)
            {
                return false;
            }

            if (order.MechanicId != mechanicId)
            {
                return false;
            }

            if (string.IsNullOrEmpty(order.MechanicId))
            {
                return false;
            }




            var mechanic = await GetMechanicHashSetAsync(db, order.MechanicId);
            if (mechanic is null || mechanic.OrderId != orderId)
            {
                return false;
            }


            mechanic.SetOrder(orderId);
            mechanic.SetMechanicStatus(MechanicStatus.Bussy);
            await CreateMechanicHashSetAsync(db, mechanic);


            // remove queue dan data untuk mechanic confirm waiting sesuai order id
            var resource = CacheKey.OrderWaitingMechanicConfirmDataPrefix(confirmation.OrderId);
            await db.KeyDeleteAsync(resource);
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId);
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);


            order.SetOrderStatus(OrderTaskStatus.MechanicAssigned);
            order.SetMechanic(mechanicId.ToString());
            await CreateOrderHashSetAsync(db, order);

            return true;
        }

        return false;
    }







      
    public async Task<bool> CompleteOrder(string orderId, string mechanicId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {  
            var order = await GetOrderTaskAsync(db, orderId);
            if (order is null)
            {
                return false;
            }

            if (order.MechanicId != mechanicId)
            {
                return false;
            }

            if (string.IsNullOrEmpty(order.MechanicId))
            {
                return false;
            }  

            var mechanic = await GetMechanicHashSetAsync(db, order.MechanicId);
            if (mechanic is null || mechanic.OrderId != orderId)
            {
                return false;
            }


            mechanic.ResetOrder();
            mechanic.SetMechanicStatus(MechanicStatus.Available);
            await CreateMechanicHashSetAsync(db, mechanic);



            var resource1 = CacheKey.OrderWaitingMechanicConfirmDataPrefix(orderId);
            await db.KeyDeleteAsync(resource1);
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId);


            var resource2 = CacheKey.OrderWaitingMechanicAssignDataPrefix(orderId);
            await db.KeyDeleteAsync(resource2);
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);

             

            return true;
        }

        return false;
    }










    public async Task<bool> RejectOrderByMechanic(string mechanicId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var waitingConfirmData = await GetOrderWaitingMechanicConfirmAsync(db, orderId);
            if (waitingConfirmData is null)
            {
                return false;
            } 

            if (waitingConfirmData.ExpiredAtUtc <= DateTime.UtcNow) // not expired already 
            {  
                return false;
            } 


            var order = await GetOrderTaskAsync(db, orderId);
            if (order is null)
            {
                return false;
            }

            if (order.MechanicId != mechanicId)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(order.MechanicId))
            {
                var mechanic = await GetMechanicHashSetAsync(db, order.MechanicId);

                if (mechanic is not null && mechanic.OrderId == orderId)
                {
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(db, mechanic);
                    await BlockMechanicToAnOrder(db, mechanic.MechanicId, orderId);
                }


                var resource1 = CacheKey.OrderWaitingMechanicConfirmDataPrefix(orderId);
                // hapus data pada mechanic waiting confirm mechanic
                await db.KeyDeleteAsync(resource1);
                // hapus queue pada mechanic waiting confirm mechanic
                await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId);



                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(db, order);
                await AddOrderFIFOtoQueue(db, orderId);

                return true;
            } 

            return false;
        }

        return false;
    }

    public async Task<bool> CancelOrderByMechanic(string mechanicId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var order = await GetOrderTaskAsync(db, orderId);
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
                var mechanic = await GetMechanicHashSetAsync(db, order.MechanicId); 
                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    // reset order pada mechanic dan setel menjadi available lagi, tapi tidak segera relesae, karena ada lock mecahnic yang tidak dihapus
                    mechanic.ResetOrder();  
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(db, mechanic);
                    // block mechanicId dari order id terkait karena mechanic membatalkan pesanan
                    await BlockMechanicToAnOrder(db, mechanic.MechanicId, orderId);
                }



                var resource1 = CacheKey.OrderWaitingMechanicConfirmDataPrefix(orderId);
                // hapus data pada mechanic waiting confirm mechanic
                await db.KeyDeleteAsync(resource1);
                // hapus queue pada mechanic waiting confirm mechanic
                await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId);





                // hapus mechanic pada order
                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(db, order);
                await AddOrderFIFOtoQueue(db, orderId);

                return true;
            }   
        } 
        return false;
    }


    public async Task<bool> CancelOrderByUser(string buyerId, string orderId)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {    
            var order = await GetOrderTaskAsync(db, orderId);
            if (order is null)
            {
                return false;
            }

            if (order.BuyerId != buyerId.ToString()) 
            {
                return false;
            }


            // MECHANIC RESET FOR ORDER ASSIGNED IF EXISTS
            if (!string.IsNullOrEmpty(order.MechanicId))
            {
                var mechanic = await GetMechanicHashSetAsync(db, order.MechanicId);
                if (mechanic is not null && mechanic.OrderId == orderId.ToString())
                {
                    // domain operation
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    // update data mechanic
                    await CreateMechanicHashSetAsync(db, mechanic); 
                    // lepaskan lock mechanic karena mechanic tidak melakukan fault
                    await DeleteKeyRedlockAsync(db, CacheKey.LockMechanicPrefix(mechanic.MechanicId));
                } 
            } 




            var resource1 = CacheKey.OrderWaitingMechanicConfirmDataPrefix(orderId);
            // hapus data pada mechanic waiting confirm mechanic
            await db.KeyDeleteAsync(resource1);
            // hapus queue pada mechanic waiting confirm mechanic
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId);


            // hapus data pada mechanic waiting assign mechanic
            var resource2 = CacheKey.OrderWaitingMechanicAssignDataPrefix(orderId);
            await db.KeyDeleteAsync(resource2);
            // hapus queue pada mechanic waiting assign mechanic
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);


            return true;
        }

        return false;
    }

    public async Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();
        var orderId = await db.ListLeftPopAsync(CacheKey.OrderWaitingMechanicConfirmQueue());

        if (string.IsNullOrEmpty(orderId) || !orderId.HasValue || orderId.IsNullOrEmpty)
        {
            return;
        }

        await TryProcessExpiryMechanicConfirmationAsync(db, orderId.ToString());
    } 


    public async Task ProcessOrdersWaitingMechanicAssignFromQueueAsync()
    {
        var db = _connectionMultiplexer.GetDatabase(); 

        var orderId = await db.ListLeftPopAsync(CacheKey.OrderWaitingMechanicAssignQueue());
        if (orderId == string.Empty || string.IsNullOrEmpty(orderId) || orderId.IsNullOrEmpty)
        {
            return;
        }

        await TryAssignMechanicToOrderAsync(db, orderId.ToString());
    }

























    private async Task TryProcessExpiryMechanicConfirmationAsync(IDatabase db, string orderId)
    {
        var key = CacheKey.LockOrderPrefix(orderId);
        using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(10));
        if (lockAcquired.IsAcquired)
        {
            var confirmation = await GetOrderWaitingMechanicConfirmAsync(db, orderId);
            if (confirmation is null)
            {
                return;
            }

            if (confirmation.ExpiredAtUtc > DateTime.UtcNow) // not expired already
            {
                await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), confirmation.OrderId);
                await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), confirmation.OrderId);
            }
            else // has expired, process it 
            {
                var resource = CacheKey.OrderWaitingMechanicConfirmDataPrefix(confirmation.OrderId);
                await db.KeyDeleteAsync(resource);



                var order = await GetOrderTaskAsync(db, confirmation.OrderId);
                if (order is null)
                {
                    return;
                }


                order.SetOrderStatus(OrderTaskStatus.WaitingMechanic);
                order.ResetMechanic();
                await CreateOrderHashSetAsync(db, order);



                var mechanic = await GetMechanicHashSetAsync(db, confirmation.MechanicId);
                if (mechanic is not null && mechanic.OrderId == confirmation.OrderId)
                {
                    mechanic.ResetOrder();
                    mechanic.SetMechanicStatus(MechanicStatus.Available);
                    await CreateMechanicHashSetAsync(db, mechanic);
                }


                await BlockMechanicToAnOrder(db, confirmation.MechanicId, orderId);

                await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), confirmation.OrderId);
                await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), confirmation.OrderId);
            }

            return;
        }

        await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId.ToString());
        await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), orderId.ToString());
    }

    private async Task TryAssignMechanicToOrderAsync(IDatabase db, string orderId)
    {
        try
        {
            var key = CacheKey.LockOrderPrefix(orderId);
            using var lockAcquired = await _redLockFactory.CreateLockAsync(key, TimeSpan.FromSeconds(30));

            if (!lockAcquired.IsAcquired)
            {
                await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
                await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
                return;
            }

            var order = await GetOrderTaskAsync(db, orderId);

            if (order is null) return;

            if (order.OrderStatus == OrderTaskStatus.OrderTaskCompleted || order.OrderStatus == OrderTaskStatus.MechanicAssigned)
            {
                return;
            }

            var mechanics = await db.GeoRadiusAsync(
                CacheKey.MechanicGeo(), order.Longitude, order.Latitude, 30, GeoUnit.Miles, count: 100, order: Order.Ascending);

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
                    var blocked = await IsMechanicBlockedFromOrder(db, mechanicId.ToString(), orderId);
                    if (blocked)
                    {
                        await DeleteKeyAsync(db, lockResource!);
                        continue;
                    }

                    var mechanicData = await GetMechanicHashSetAsync(db, mechanicId.ToString());
                    if (mechanicData is null)
                    {
                        await DeleteKeyAsync(db, lockResource!);
                        continue;
                    }

                    if (mechanicData.Status is MechanicStatus.Available)
                    {
                        order.SetOrderStatus(OrderTaskStatus.MechanicAssigned);
                        order.SetMechanic(mechanicId);
                        await CreateOrderHashSetAsync(db, order);


                        mechanicData.SetOrder(orderId);
                        mechanicData.SetMechanicStatus(MechanicStatus.Bussy);
                        await CreateMechanicHashSetAsync(db, mechanicData);


                        await OrderWaitingConfirmMechanic(db, order, mechanicData);
                        return;
                    }

                    if (mechanicData.Status is MechanicStatus.Unavailable || mechanicData.Status is MechanicStatus.Bussy)
                    {
                        await DeleteKeyAsync(db, lockResource!);
                    }
                }
            }
             
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
            await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
        }
        catch (Exception)
        {
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
            await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
        }
    }
     

    private async Task AddOrderFIFOtoQueue(IDatabase db, string orderId)
    {
        try
        {
            await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
            await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicAssignQueue(), orderId);
        }
        catch (Exception)
        {
            throw;
        }
    }



    private async Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic)
    { 
        await db.ListRemoveAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), order.OrderId);
        await db.ListRightPushAsync(CacheKey.OrderWaitingMechanicConfirmQueue(), order.OrderId);

        var resource = CacheKey.OrderWaitingMechanicConfirmDataPrefix(order.OrderId);  
        var hashEntries = new HashEntry[]
        {
            new (nameof(OrderTaskMechanicConfirm.OrderId), order.OrderId),
            new (nameof(OrderTaskMechanicConfirm.MechanicId), mechanic.MechanicId),
            new (nameof(OrderTaskMechanicConfirm.ExpiredAtUtc), DateTime.UtcNow.AddSeconds(180).ToString())
        };

        await db.HashSetAsync(resource, hashEntries); 
    }

    private async Task<bool> IsMechanicBlockedFromOrder(IDatabase db, string mechanicId, string orderId)
    { 
        var setKey = CacheKey.MechanicOrderBlacklistPrefix(mechanicId);
        var isBlocked = await db.SetContainsAsync(setKey, orderId);

        return isBlocked;
    }

    private async Task BlockMechanicToAnOrder(IDatabase db, string mechanicId, string orderId)
    {
        try
        { 
            var setKey = CacheKey.MechanicOrderBlacklistPrefix(mechanicId);
            await db.SetAddAsync(setKey, orderId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }



    private async Task<(bool isAcquired, string? resourceLock)> AcquireLockAsync(string mechanicId)
    {
        var lockResource = CacheKey.LockMechanicPrefix(mechanicId);
        var mechanicLock = await _redLockFactory.CreateLockAsync(lockResource, TimeSpan.FromMinutes(10));

        if (mechanicLock.IsAcquired)
        {
            return (true, lockResource);
        }

        return (false, null);
    }

    private async Task DeleteKeyRedlockAsync(IDatabase db, string lockResource)
    {
        await db.KeyDeleteAsync(CacheKey.RedlockPrefix(lockResource));
    }

    private async Task DeleteKeyAsync(IDatabase db, string lockResource)
    {
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

    private async Task<MechanicExistence?> GetMechanicHashSetAsync(IDatabase db, string mechanicId)
    {
        var hashKey = CacheKey.MechanicExistencePrefix(mechanicId);
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;


        var latitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Latitude)).Value);
        var longitude = RedisValueToDouble(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Longitude)).Value);
        var orderId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.OrderId)).Value);
        var status = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(MechanicExistence.Status)).Value);



        return new MechanicExistence(mechanicId.ToString(), orderId, latitude, longitude, status);
    }


    private async Task<OrderTask?> GetOrderTaskAsync(IDatabase db, string orderId)
    {
        var hashKey = CacheKey.OrderWaitingMechanicAssignDataPrefix(orderId);
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

    private async Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(IDatabase db, string orderId)
    {
        var hashKey = CacheKey.OrderWaitingMechanicConfirmDataPrefix(orderId);
        var hashEntries = await db.HashGetAllAsync(hashKey);

        if (hashEntries.Length == 0)
            return null;

        var mechanicId = RedisValueToString(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTaskMechanicConfirm.MechanicId)).Value);

        var expireAtUtc = RedisValueToDateTime(hashEntries.FirstOrDefault(x => x.Name == nameof(OrderTaskMechanicConfirm.ExpiredAtUtc)).Value);

        return new OrderTaskMechanicConfirm(orderId.ToString(), mechanicId, expireAtUtc);
    }


    private async Task CreateGeoAsync(IDatabase db, MechanicExistence mechanic)
    {
        try
        {
            var geoKey = CacheKey.MechanicGeo();
            await db.GeoAddAsync(geoKey, mechanic.Longitude, mechanic.Latitude, mechanic.MechanicId.ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task CreateMechanicHashSetAsync(IDatabase db, MechanicExistence mechanic)
    {
        try
        {
            var hashKey = CacheKey.MechanicExistencePrefix(mechanic.MechanicId);
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

    private async Task CreateOrderHashSetAsync(IDatabase db, OrderTask order)
    {
        try
        {
            var hashKey = CacheKey.OrderWaitingMechanicAssignDataPrefix(order.OrderId);
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
     
}

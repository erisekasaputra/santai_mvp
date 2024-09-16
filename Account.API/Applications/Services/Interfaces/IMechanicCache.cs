

using Account.API.Applications.Models;
using StackExchange.Redis;

namespace Account.API.Applications.Services.Interfaces;

public interface IMechanicCache
{
    Task<bool> UpdateLocationAsync(MechanicExistence mechanic);
    Task<MechanicExistence?> GetMechanicHashSetAsync(string mechanicId);
    Task<OrderTask?> GetOrderTaskAsync(string orderId);
    Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(string orderId);
    Task CreateGeoAsync(MechanicExistence mechanic);
    Task CreateMechanicHashSetAsync(MechanicExistence mechanic);
    Task CreateOrderHashSetAsync(OrderTask order);
    Task<bool> Activate(string mechanicId);
    Task<bool> Deactivate(string mechanicId);
    Task<bool> PingAsync();
    Task CreateOrderToQueueAndHash(OrderTask orderTask);
    Task<bool> AcceptOrderByMechanic(string orderId, string mechanicId);
    Task<bool> RejectOrderByMechanic(string mechanicId, string orderId);
    Task<bool> CancelOrderByMechanic(string mechanicId, string orderId);
    Task<bool> CancelOrderByUser(string buyerId, string orderId);
    Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync(); 
    Task ProcessOrdersWaitingMechanicAssignFromQueueAsync();
    Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic);
    Task<bool> IsMechanicBlockedFromOrder(string mechanicId, string orderId);
}

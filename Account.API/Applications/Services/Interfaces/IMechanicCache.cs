

using Account.API.Applications.Models;
using StackExchange.Redis;

namespace Account.API.Applications.Services.Interfaces;

public interface IMechanicCache
{
    Task<bool> UpdateLocationAsync(MechanicExistence mechanic);
    Task<MechanicExistence?> GetMechanicHashSetAsync(Guid mechanicId);
    Task<OrderTask?> GetOrderTaskAsync(Guid orderId);
    Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(Guid orderId);
    Task CreateGeoAsync(MechanicExistence mechanic);
    Task CreateMechanicHashSetAsync(MechanicExistence mechanic);
    Task CreateOrderHashSetAsync(OrderTask order);
    Task<bool> Activate(Guid mechanicId);
    Task<bool> Deactivate(Guid mechanicId);
    Task<bool> PingAsync();
    Task CreateOrderToQueueAndHash(OrderTask orderTask);
    Task<bool> AcceptOrderByMechanic(Guid orderId, Guid mechanicId);
    Task<bool> RejectOrderByMechanic(Guid mechanicId, Guid orderId);
    Task<bool> CancelOrderByMechanic(Guid mechanicId, Guid orderId);
    Task<bool> CancelOrderByUser(Guid buyerId, Guid orderId);
    Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync(); 
    Task ProcessOrdersWaitingMechanicAssignFromQueueAsync();
    Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic);
    Task<bool> IsMechanicBlockedFromOrder(Guid mechanicId, Guid orderId);
}

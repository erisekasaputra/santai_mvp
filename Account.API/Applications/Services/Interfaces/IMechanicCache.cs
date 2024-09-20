using Account.API.Applications.Models; 

namespace Account.API.Applications.Services.Interfaces;

public interface IMechanicCache
{
    Task<bool> UpdateLocationAsync(MechanicExistence mechanic); 
    Task<bool> Activate(string mechanicId);
    Task<bool> Deactivate(string mechanicId);
    Task<bool> PingAsync();
    Task CreateOrderToQueueAndHash(OrderTask orderTask);
    Task ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync(); 
    Task<(bool isSuccess, string orderId, string buyerId, string mechanicId)> ProcessOrdersWaitingMechanicAssignFromQueueAsync(); 


    Task<(bool isSuccess, string buyerId)> AcceptOrderByMechanic(string orderId, string mechanicId);
    Task<(bool isSuccess, string buyerId)> RejectOrderByMechanic(string orderId, string mechanicId);
    Task<(bool isSuccess, string buyerId)> CancelOrderByMechanic(string orderId, string mechanicId);
    Task<(bool isSuccess, string buyerId)> CompleteOrder(string orderId, string mechanicId); 
    Task<(bool isSuccess, string mechanicId)> CancelOrderByUser(string orderId, string buyerId);

    
    
    
    
    //Task OrderWaitingConfirmMechanic(IDatabase db, OrderTask order, MechanicExistence mechanic);
    //Task<bool> IsMechanicBlockedFromOrder(string mechanicId, string orderId);    
    //Task<MechanicExistence?> GetMechanicHashSetAsync(string mechanicId);
    //Task<OrderTask?> GetOrderTaskAsync(string orderId);
    //Task<OrderTaskMechanicConfirm?> GetOrderWaitingMechanicConfirmAsync(string orderId);
    //Task CreateGeoAsync(MechanicExistence mechanic);
    //Task CreateMechanicHashSetAsync(MechanicExistence mechanic);
    //Task CreateOrderHashSetAsync(OrderTask order);
}

namespace Notification.Worker.Services.Interfaces;

public interface IActivityClient
{
    Task ReceiveOrderStatusUpdate(
        string orderId, 
        string buyerId,
        string buyerName,
        string mechanicId,
        string mechanicName,
        string orderStatus,
        string actionUrl);
}

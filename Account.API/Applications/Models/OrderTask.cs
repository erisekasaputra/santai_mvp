namespace Account.API.Applications.Models;

public class OrderTask
{
    public string OrderId { get; private init; }
    public string BuyerId { get; private init; }
    public string MechanicId { get; private set; }
    public double Latitude { get; private init; }
    public double Longitude { get; private init; }
    public string OrderStatus { get; private set; }

    public OrderTask(
        string orderId, 
        string buyerId,
        string mechanicId,
        double latitude,
        double longitude,
        string orderStatus)
    {
        if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException(nameof(orderId));   
        if (string.IsNullOrEmpty(buyerId)) throw new ArgumentNullException(nameof(buyerId));   
        if (mechanicId is null) throw new ArgumentNullException(nameof(mechanicId));   
        if (string.IsNullOrEmpty(orderStatus)) throw new ArgumentNullException(nameof(orderStatus));   

        OrderId = orderId;
        BuyerId = buyerId;
        MechanicId = mechanicId;
        Latitude = latitude;
        Longitude = longitude;
        OrderStatus = orderStatus;
    }

    public void SetMechanic(string mechanicId)
    { 
        if (string.IsNullOrEmpty(mechanicId)) throw new ArgumentNullException(mechanicId);
        MechanicId = mechanicId;
    }

    public void ResetMechanic()
    {
        if (string.IsNullOrEmpty(MechanicId))
        {
            return;
        } 
        MechanicId = string.Empty;
        OrderStatus = OrderTaskStatus.WaitingMechanic;
    }

    public void SetOrderStatus(string status)
    {
        if (string.IsNullOrEmpty(status)) throw new ArgumentNullException(status); 
        OrderStatus = status;
    }
}

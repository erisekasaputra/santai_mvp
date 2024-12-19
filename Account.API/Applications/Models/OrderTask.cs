namespace Account.API.Applications.Models;

public class OrderTask
{
    public string OrderId { get; private init; }
    public string BuyerId { get; private init; }
    public string MechanicId { get; private set; }
    public string MechanicName { get; private set; }
    public string MechanicImageUrl { get; private set; }
    public double Latitude { get; private init; }
    public double Longitude { get; private init; }
    public string OrderStatus { get; private set; }

    public OrderTask(
        string orderId, 
        string buyerId,
        string mechanicId,
        string mechanicName,
        string mechanicImageUrl,
        double latitude,
        double longitude,
        string orderStatus)
    {
        if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException(nameof(orderId));   
        if (string.IsNullOrEmpty(buyerId)) throw new ArgumentNullException(nameof(buyerId));
        ArgumentNullException.ThrowIfNull(mechanicId);
        ArgumentNullException.ThrowIfNull(mechanicName);  
        if (string.IsNullOrEmpty(orderStatus)) throw new ArgumentNullException(nameof(orderStatus));   

        OrderId = orderId;
        BuyerId = buyerId;
        MechanicId = mechanicId;
        MechanicName = mechanicName;
        MechanicImageUrl = mechanicImageUrl;
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

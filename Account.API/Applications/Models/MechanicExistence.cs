namespace Account.API.Applications.Models;

public class MechanicExistence
{
    public string MechanicId { get; private init; }   
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string OrderId { get; private set; } 
    public string Status { get; private set; }

    public MechanicExistence(
        string mechanicId,
        string orderId,
        double latitude,
        double longitude,
        string status)
    { 
        MechanicId = mechanicId ?? throw new ArgumentNullException(mechanicId);
        Latitude = latitude;
        Longitude = longitude;
        OrderId = orderId ?? throw new ArgumentNullException(mechanicId);
        Status = status ?? throw new ArgumentNullException(mechanicId);
    }

    public void SetOrder(string orderId)
    { 
        OrderId = orderId ?? throw new ArgumentNullException(orderId);
    }

    public void ResetOrder()
    {
        if (string.IsNullOrEmpty(OrderId))
        {
            return;
        }

        OrderId = string.Empty;
    }

    public void SetMechanicStatus(string status)
    {   
        Status = status ?? throw new ArgumentNullException(status);
    }
}

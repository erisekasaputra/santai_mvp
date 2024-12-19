namespace Account.API.Applications.Models;

public class MechanicExistence(
    string mechanicId,
    string name,
    string imageUrl,
    string orderId,
    double latitude,
    double longitude,
    string status)
{
    public string MechanicId { get; private init; } = mechanicId ?? throw new ArgumentNullException(mechanicId);
    public string Name { get; private init; } = name ?? throw new ArgumentNullException(name);
    public string ImageUrl { get; private set; } = imageUrl;
    public double Latitude { get; private set; } = latitude;
    public double Longitude { get; private set; } = longitude;
    public string OrderId { get; private set; } = orderId ?? throw new ArgumentNullException(mechanicId);
    public string Status { get; private set; } = status ?? throw new ArgumentNullException(mechanicId);

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

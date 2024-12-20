namespace Core.Events.Models;

public class OrderFleetEventModel
{
    public Guid Id { get; set; }
    public Guid FleetId { get; set; }
    public Guid OrderId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string? RegistrationNumber { get; set; }
    public string ImageUrl { get; set; }
    
    public OrderFleetEventModel(
        Guid id,
        Guid fleetId,
        Guid orderId,
        string brand,
        string model,
        string? registrationNumber,
        string imageUrl)
    {
        Id = id;
        FleetId = fleetId;
        OrderId = orderId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}

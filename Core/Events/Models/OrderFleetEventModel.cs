namespace Core.Events.Models;

public class OrderFleetEventModel
{
    public Guid Id { get; set; }
    public Guid FleetId { get; set; }
    public Guid OrderId { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public string? ImageUrl { get; private set; }
    
    public OrderFleetEventModel(
        Guid id,
        Guid orderingId,
        Guid fleetId,
        string brand,
        string model,
        string registrationNumber,
        string? imageUrl)
    {
        Id = id;
        OrderId = orderingId;
        FleetId = fleetId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}

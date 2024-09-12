using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Fleet : Entity
{
    public Guid FleetId { get; set; }
    public Guid OrderId { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public string ImageUrl { get; private set; }
    public Fleet()
    {
        Brand = string.Empty;
        Model = string.Empty;
        RegistrationNumber = string.Empty;
        ImageUrl = string.Empty;
    }
    public Fleet(
        Guid orderingId,
        Guid fleetId,
        string brand,
        string model,
        string registrationNumber,
        string imageUrl)
    {
        OrderId = orderingId;
        FleetId = fleetId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}

using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.OrderAggregate;

public class Fleet : Entity
{
    public Guid OrderingId { get; private set; }
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
    public Fleet(Guid orderingId, Guid fleetId, string brand, string model, string registrationNumber, string imageUrl)
    {
        OrderingId = orderingId;
        Id = fleetId;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}

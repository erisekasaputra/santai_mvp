using Order.Domain.SeedWork;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Fleet : Entity
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public string ImageUrl { get; private set; }
    public Fleet(Guid id, string brand, string model, string registrationNumber, string imageUrl)
    {
        Id = id;
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
        ImageUrl = imageUrl;
    }
}

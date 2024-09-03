using Order.Domain.SeedWork;

namespace Order.Domain.Aggregates.OrderAggregate;

public class Fleet : Entity
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string RegistrationNumber { get; private set; }
    public Fleet(string brand, string model, string registrationNumber)
    {
        Brand = brand;
        Model = model;
        RegistrationNumber = registrationNumber;
    }
}

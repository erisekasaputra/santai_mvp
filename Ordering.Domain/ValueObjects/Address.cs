using Ordering.Domain.SeedWork;

namespace Ordering.Domain.ValueObjects;

public class Address : ValueObject
{
    public string AddressLine { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Address(string addressLine, double latitude, double longitude)
    {
        AddressLine = addressLine;
        Latitude = latitude;
        Longitude = longitude;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return Latitude;
        yield return Longitude;
    }
}

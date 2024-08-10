using Account.Domain.SeedWork;

namespace Account.Domain.ValueObjects;

public class Address : ValueObject
{
    public string AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? AddressLine3 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
     
    public Address(string addressLine1, string? addressLine2, string? addressLine3, string city, string state, string postalCode, string country)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    } 

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AddressLine1;
        yield return AddressLine2;
        yield return AddressLine3;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
}

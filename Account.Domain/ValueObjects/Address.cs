using Account.Domain.SeedWork;

namespace Account.Domain.ValueObjects;

public class Address : ValueObject
{
    public string EncryptedAddressLine1 { get; private set; }
    public string? EncryptedAddressLine2 { get; private set; }
    public string? EncryptedAddressLine3 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
     
    public Address(string encryptedAddressLine1, string? encryptedAddressLine2, string? encryptedAddressLine3, string city, string state, string postalCode, string country)
    {
        EncryptedAddressLine1 = encryptedAddressLine1;
        EncryptedAddressLine2 = encryptedAddressLine2;
        EncryptedAddressLine3 = encryptedAddressLine3;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    } 

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EncryptedAddressLine1;
        yield return EncryptedAddressLine2;
        yield return EncryptedAddressLine3;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
}

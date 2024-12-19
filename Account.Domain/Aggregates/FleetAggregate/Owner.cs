using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.FleetAggregate;

public class Owner : ValueObject
{
    public string? EncryptedOwnerName { get; private set; }  
    public string? EncryptedOwnerAddress { get; private set; } 


    public Owner(string? encryptedOwnerName, string? encryptedOwnerAddress)
    {
        EncryptedOwnerName = string.IsNullOrEmpty(encryptedOwnerName) ? null : encryptedOwnerName;
        EncryptedOwnerAddress = string.IsNullOrEmpty(encryptedOwnerAddress) ? null : encryptedOwnerAddress; 
    }

    public void Update(string? encryptedOwnerName, string? encryptedOwnerAddress)
    {
        EncryptedOwnerName = string.IsNullOrEmpty(encryptedOwnerName) ? null : encryptedOwnerName;
        EncryptedOwnerAddress = string.IsNullOrEmpty(encryptedOwnerAddress) ? null : encryptedOwnerAddress;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EncryptedOwnerAddress;
        yield return EncryptedOwnerName;
    }
}

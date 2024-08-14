using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.FleetAggregate;

public class Owner : ValueObject
{
    public string EncryptedOwnerName { get; private set; }

    public string EncryptedOwnerAddress { get; private set; } 


    public Owner(string encryptedOwnerName, string encryptedOwnerAddress)
    {
        EncryptedOwnerName = encryptedOwnerName;
        EncryptedOwnerAddress = encryptedOwnerAddress; 
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EncryptedOwnerAddress;
        yield return EncryptedOwnerName;
    }
}

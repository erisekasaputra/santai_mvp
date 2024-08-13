namespace Vehicle.API.Domain.Entities;

public class Owner
{
    public string EncryptedOwnerName { get; private set; }

    public string EncryptedOwnerAddress { get; private set; }

    public string EncryptedOwnerPhoneNumber { get; private set; }

    public Owner(string encryptedOwnerName, string encryptedOwnerAddress, string encryptedOwnerPhoneNumber)
    {
        EncryptedOwnerName = encryptedOwnerName;
        EncryptedOwnerAddress = encryptedOwnerAddress;
        EncryptedOwnerPhoneNumber = encryptedOwnerPhoneNumber;
    }
}

namespace Order.API.Applications.Services.Interfaces;

public interface IKeyManagementService
{
    Task<string> EncryptAsync(string plainText);
    Task<string> DecryptAsync(string cipherText);
}

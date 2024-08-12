namespace Account.API.Services;

public interface IKeyManagementService
{
    Task<string> EncryptAsync(string plainText);
    Task<string> DecryptAsync(string cipherText); 
}

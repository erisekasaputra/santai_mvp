using Account.API.Applications.Services.Interfaces;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using System.Text;

namespace Account.API.Applications.Services;

public class CloudKeyManagementService : IKeyManagementService
{
    private readonly IAmazonKeyManagementService _kmsClient;
    private readonly string _keyId;
    public CloudKeyManagementService(IAmazonKeyManagementService kmsClient, string keyId)
    {
        _kmsClient = kmsClient;
        _keyId = keyId;
    }

    public async Task<string> DecryptAsync(string cipherText)
    {
        var request = new DecryptRequest
        {
            CiphertextBlob = new MemoryStream(Encoding.UTF8.GetBytes(cipherText))
        };

        var response = await _kmsClient.DecryptAsync(request);
        return Convert.ToBase64String(response.Plaintext.ToArray());
    }

    public async Task<string> EncryptAsync(string plainText)
    {
        var request = new EncryptRequest
        {
            KeyId = _keyId,
            Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plainText))
        };

        var response = await _kmsClient.EncryptAsync(request);
        return Convert.ToBase64String(response.CiphertextBlob.ToArray());
    }
}

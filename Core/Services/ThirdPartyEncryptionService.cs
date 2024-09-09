using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Core.Services.Interfaces;
using System.Text;

namespace Core.Services;

public class ThirdPartyEncryptionService : IEncryptionService
{
    private readonly IAmazonKeyManagementService _kmsClient;
    private readonly string _keyId;
    public ThirdPartyEncryptionService(IAmazonKeyManagementService kmsClient, string keyId)
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

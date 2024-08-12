using System.Security.Cryptography;
using System.Text;

namespace Account.API.Services;

public class LocalKeyManagementService : IKeyManagementService
{
    private readonly byte[] _key;

    public LocalKeyManagementService(string keyId)
    {
        _key = HashKey(keyId);  
    }

    private byte[] HashKey(string key)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    public async Task<string> DecryptAsync(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        var iv = fullCipher.Take(16).ToArray();
        var cipher = fullCipher.Skip(16).ToArray();

        using var aes = Aes.Create(); 
        aes.Key = _key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return await Task.FromResult(sr.ReadToEnd());
    }
     
    public async Task<string> EncryptAsync(string plainText)
    {
        using var aes = Aes.Create(); 
        var iv = aes.IV;
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        var encrypted = ms.ToArray();
        var result = iv.Concat(encrypted).ToArray();
        return await Task.FromResult(Convert.ToBase64String(result));
    } 
     
}

using Core.Configurations;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Core.Services;

public class HashService : IHashService
{
    private readonly byte[] _key;
    public HashService(IOptionsMonitor<EncryptionConfiguration> options)
    {
        _key = Encoding.UTF8.GetBytes(options.CurrentValue.SecretKey);
    }
    public async Task<string> Hash(string input)
    {
        using var hmac = new HMACSHA256(_key);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return await Task.FromResult(Convert.ToBase64String(hashBytes));
    }
}

using System.Security.Cryptography;
using System.Text;

namespace Account.API.Utilities;

public static class UniqueIdGenerator
{
    public static string Generate(Guid id, int length = 6)
    {
        string referrerIdString = id.ToString();

        using var sha256 = SHA256.Create();

        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(referrerIdString));

        string base64String = Convert.ToBase64String(hashBytes);

        string cleanedBase64 = base64String.Replace("/", "").Replace("+", "").Replace("=", "");

        return cleanedBase64[..Math.Min(length, cleanedBase64.Length)].ToUpper();
    }
}
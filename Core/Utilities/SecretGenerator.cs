using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities;

public static class SecretGenerator
{
    public static string GenerateRandomSecret()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static string HashToken(this string token)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }

    public static string HmacHash(this string value, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)); 
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexStringLower(hash);
    }
      

    public static string GenerateOtp(int length = 6)
    {
        const string digits = "0123456789";
        var otp = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomNumber = new byte[1];
            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(randomNumber);
                int randomIndex = randomNumber[0] % digits.Length;
                otp[i] = digits[randomIndex];
            }
        }
        return new string(otp);
    }
}

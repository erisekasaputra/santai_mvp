
namespace Identity.API.SeedWork;

public class CacheKey
{
    private const string prefixRefreshToken = "RefreshToken";
    private const string prefixOtp = "Otp";
    private const string prefixRequestOtp = "RequestOtp";
    public static string RefreshTokenCacheKey(string userId) => $"{prefixRefreshToken}#{userId}"; 
    public static string OtpCacheKey(string phoneNumber) => $"{prefixOtp}#{phoneNumber}";
    public static string RequestOtpCacheKey(string phoneNumber) => $"{prefixRequestOtp}#{phoneNumber}";
}

namespace Identity.API.SeedWork;

public class CacheKey
{
    private const string prefixRefreshToken = "RefreshToken";
    private const string prefixOtp = "Otp";
    private const string prefixRequestOtp = "RequestOtp";
    private const string prefixBlackListAccessToken = "BlacklistAccessToken";
    private const string prefixBlackListRefreshToken = "BlacklistRefreshToken";
    public static string RefreshTokenCacheKey(string userId) => $"{prefixRefreshToken}#{userId}"; 
    public static string OtpCacheKey(string phoneNumber) => $"{prefixOtp}#{phoneNumber}";
    public static string RequestOtpCacheKey(string phoneNumber) => $"{prefixRequestOtp}#{phoneNumber}";
    public static string BlackListAccessTokenKey(string token) => $"{prefixBlackListAccessToken}#{token}";
    public static string BlackListRefreshTokenKey(string token) => $"{prefixBlackListRefreshToken}#{token}";
}

namespace Core.SeedWorks;

public class CacheKey
{
    private const string prefixOrderServiceToken = "OrderServiceTokenCache";
    private const string prefixRefreshToken = "RefreshToken";
    private const string prefixOtp = "OtpRequested";
    private const string prefixRequestOtp = "GetOtpRequest";
    private const string prefixBlackListAccessToken = "BlacklistAccessToken";
    private const string prefixBlackListRefreshToken = "BlacklistRefreshToken";
    public static string OrderServiceCacheKey() => $"{prefixOrderServiceToken}";
    public static string RefreshTokenCacheKey(string userId) => $"{prefixRefreshToken}#{userId}";
    public static string OtpCacheKey(string phoneNumber) => $"{prefixOtp}#{phoneNumber}";
    public static string RequestOtpCacheKey(string phoneNumber) => $"{prefixRequestOtp}#{phoneNumber}";
    public static string BlackListAccessTokenKey(string token) => $"{prefixBlackListAccessToken}#{token}";
    public static string BlackListRefreshTokenKey(string token) => $"{prefixBlackListRefreshToken}#{token}";
}

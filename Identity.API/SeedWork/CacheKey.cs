using Newtonsoft.Json.Linq;

namespace Identity.API.SeedWork;

public class CacheKey
{
    private const string prefixRefreshToken = "IdentityService:RefreshToken";
    private const string prefixOtp = "IdentityService:OtpRequested";
    private const string prefixRequestOtp = "IdentityService:GetOtpRequest";
    private const string prefixBlackListAccessToken = "IdentityService:BlacklistAccessToken";
    private const string prefixBlackListRefreshToken = "IdentityService:BlacklistRefreshToken";
    public static string RefreshTokenCacheKey(string token) => $"{prefixRefreshToken}#{token}";
    public static string OtpCacheKey(string phoneNumber) => $"{prefixOtp}#{phoneNumber}";
    public static string RequestOtpCacheKey(string phoneNumber) => $"{prefixRequestOtp}#{phoneNumber}";
    public static string BlackListAccessTokenKey(string token) => $"{prefixBlackListAccessToken}#{token}";
    public static string BlackListRefreshTokenKey(string token) => $"{prefixBlackListRefreshToken}#{token}";
}

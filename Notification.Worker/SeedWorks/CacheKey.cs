namespace Notification.Worker.SeedWorks;

public static class CacheKey
{
    public const string UserCacheKeyPrefix = "Account:Users";
    public static string GetUserCacheKey(string userId) => $"{UserCacheKeyPrefix}:{userId}";
}

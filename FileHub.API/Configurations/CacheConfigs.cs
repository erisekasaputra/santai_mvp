namespace FileHub.API.Configurations;

public class CacheConfigs
{
    public const string SectionName = "CacheConfigs";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ConnectTimeout { get; set; }
    public int SyncTimeout { get; set; }
    public int ReconnectRetryPolicy { get; set; }
    public int CacheLifeTime { get; set; }
}

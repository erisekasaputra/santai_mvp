using MassTransit.Saga;

namespace Core.Configurations;

public class CacheConfiguration
{ 
    public const string SectionName = "Cache";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ConnectTimeout { get; set; }
    public int SyncTimeout { get; set; }
    public int ReconnectRetryPolicy { get; set; }
    public int CacheLifeTime { get; set; }
}

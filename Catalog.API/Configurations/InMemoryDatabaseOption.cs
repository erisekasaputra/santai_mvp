namespace Catalog.API.Configurations;

public class InMemoryDatabaseOption
{
    public const string SectionName = "InMemoryDatabase";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ConnectTimeout { get; set; }
    public int SyncTimeout { get; set; }
    public int ReconnectRetryPolicy { get; set; }
    public int CacheLifeTime { get; set; }
}

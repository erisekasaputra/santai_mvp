namespace Identity.API.Configs;

public class DatabaseConfig 
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; }
    public int MaxRetryCount { get; set; }
    public int MaxRetryDelay { get; set; }
}

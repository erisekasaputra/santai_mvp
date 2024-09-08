namespace Core.Configurations;

public class DatabaseConfiguration
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = default!;
    public int CommandTimeout { get; set; } = default!;
    public int MaxRetryCount { get; set; } = default!;
}

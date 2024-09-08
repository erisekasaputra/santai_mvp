namespace Catalog.API.Configurations;

public class DatabaseConfiguration
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeOut { get; set; }
}

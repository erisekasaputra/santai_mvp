namespace Identity.API.Configs;

public class DatabaseConfig 
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
}

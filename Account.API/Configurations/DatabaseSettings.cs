namespace Account.API.Configurations;

public class DatabaseSettings
{
    public const string DatabaseSection = "Database";

    public string ConnectionString { get; set; } = default!;

    public int CommandTimeOut { get; set; } = default!;
}

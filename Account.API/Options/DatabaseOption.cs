namespace Account.API.Options;

public class DatabaseOption
{
    public const string SectionName = "Database"; 
    public string ConnectionString { get; set; } = default!; 
    public int CommandTimeOut { get; set; } = default!;
}

namespace Account.API.Options;

public class InMemoryDatabaseOption
{
    public const string SectionName = "InMemoryDatabase"; 
    public string Host { get; set; } = string.Empty; 
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

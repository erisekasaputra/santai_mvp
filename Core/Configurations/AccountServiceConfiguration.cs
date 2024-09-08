namespace Core.Configurations;

public class AccountServiceConfiguration
{
    public const string SectionName = "AccountService";
    public string Host { get; set; } = string.Empty;
    public string Accept { get; set; } = string.Empty;
}

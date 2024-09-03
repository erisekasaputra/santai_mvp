namespace Order.API.Configurations;

public class AccountServiceClientConfiguration
{
    public const string SectionName = "AccountServiceClient";
    public string Host { get; set; } = string.Empty; 
    public string Accept { get; set; } = string.Empty;
}

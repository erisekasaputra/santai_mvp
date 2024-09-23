namespace Core.Configurations;

public class GoogleSSOConfiguration
{
    public const string SectionName = "Google";
    public string ClientID { get; set; } = string.Empty; 
    public string ClientSecret { get; set; } = string.Empty;
}

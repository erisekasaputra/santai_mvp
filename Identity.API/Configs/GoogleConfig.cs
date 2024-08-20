namespace Identity.API.Configs;

public class GoogleConfig
{
    public const string SectionName = "Google";
    public string ClientId { get; set; } = string.Empty; 
    
    // from user-secrets dotnet feature // need to see installation instruction
    public string ClientSecret { get; set; } = string.Empty; 
}

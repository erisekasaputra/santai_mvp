namespace Identity.API.Configs;

public class FacebookConfig
{
    public const string SectionName = "Facebook";
    public string AppId { get; set; } = string.Empty;

    // from user-secrets dotnet feature // need to see installation instruction
    public string AppSecret { get; set; } = string.Empty;
}

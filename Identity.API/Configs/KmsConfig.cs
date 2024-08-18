namespace Identity.API.Configs;

public class KmsConfig
{
    public const string SectionName = "KMS";
    public string SecretKey { get; set; } = string.Empty;
}

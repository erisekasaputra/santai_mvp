namespace FileHub.API.Configurations;
 
public class KmsConfigs
{
    public const string SectionName = "KMS";
    public string SecretKey { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

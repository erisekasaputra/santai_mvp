namespace Core.Configurations;

public class EncryptionConfiguration
{
    public const string SectionName = "Encryption";
    public string SecretKey { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

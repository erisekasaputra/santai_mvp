namespace Order.API.Configurations;

public class KeyManagementServiceOption
{
    public const string SectionName = "KMS";
    public string SecretKey { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

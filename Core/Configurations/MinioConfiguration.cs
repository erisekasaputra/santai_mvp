namespace Core.Configurations;

public class MinioConfiguration
{
    public const string SectionName = "Minio";
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }
}

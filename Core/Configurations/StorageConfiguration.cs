namespace Core.Configurations;

public class StorageConfiguration
{
    public const string SectionName = "Storage";
    public string AWS_ACCESS_KEY_ID { get; set; } = string.Empty;
    public string AWS_SECRET_ACCESS_KEY { get; set; } = string.Empty;
    public string AWS_DEFAULT_REGION { get; set; } = string.Empty;
    public string? BucketPrivate { get; set; }
    public string? BucketPublic { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }
    public string? CdnServiceUrl { get; set; }
    public string? Region { get; set; }
    public bool UseMinio { get; set; }
    public int MultipartBodyLengthLimit { get; set; }
}

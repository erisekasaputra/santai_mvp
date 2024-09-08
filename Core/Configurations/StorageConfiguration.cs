namespace Core.Configurations;

public class StorageConfiguration
{
    public const string SectionName = "Storage";
    public string? BucketPrivate { get; set; }
    public string? BucketPublic { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }
    public string? CdnServiceUrl { get; set; }
    public string? Region { get; set; }
    public bool UseMinio { get; set; }
}

namespace Core.Configurations;

public class StorageConfiguration
{
    public const string SectionName = "Storage";
    public string? BucketPrivate { get; set; }
    public string? BucketPublic { get; set; }
    public string? CdnServiceUrl { get; set; } 
    public bool UseMinio { get; set; }
    public int MultipartBodyLengthLimit { get; set; }
}

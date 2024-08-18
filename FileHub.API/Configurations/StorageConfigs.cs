namespace FileHub.API.Configurations;

public class StorageConfigs
{
    public const string SectionName = "StorageConfigs";
    public string? BucketPrivate { get; set; }
    public string? BucketPublic { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }  
    public string? CdnServiceUrl { get; set; }   
    public string? Region { get; set; }
    public bool UseMinio { get; set; } 
}

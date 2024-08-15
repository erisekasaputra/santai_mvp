namespace FileHub.API.Configurations;

public class StorageConfigs
{
    public const string SectionName = "StorageConfigs";
    public string? BucketName { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ServiceUrl { get; set; }  
    public bool UseMinio { get; set; } 
}

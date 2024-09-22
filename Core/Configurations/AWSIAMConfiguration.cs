namespace Core.Configurations;

public class AWSIAMConfiguration
{
    public const string SectionName = "AWS";
    public string AccessID { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;  
}

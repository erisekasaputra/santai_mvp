namespace Core.Configurations; 

public class CatalogServiceConfiguration
{
    public const string SectionName = "CatalogService";
    public string Host { get; set; } = string.Empty;
    public string Accept { get; set; } = string.Empty;
}
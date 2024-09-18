namespace Core.Configurations;

public class MasterDataServiceConfiguration
{
    public const string SectionName = "MasterDataService";
    public string Host { get; set; } = string.Empty;
    public string Accept { get; set; } = string.Empty;
}

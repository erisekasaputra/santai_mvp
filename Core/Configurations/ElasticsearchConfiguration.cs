namespace Core.Configurations;

public class ElasticsearchConfiguration
{
    public const string SectionName = "Elasticsearch";
    public List<string> Addresses { get; set; } = [];
}

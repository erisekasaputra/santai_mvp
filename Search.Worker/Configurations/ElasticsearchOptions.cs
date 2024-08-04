namespace Search.Worker.Configurations;

public class ElasticsearchOptions
{
    public List<string> Addresses { get; set; } 
    
    public ElasticsearchOptions()
    {
        Addresses = [];
    }
}

namespace Search.Worker.SeedWork;

public class ElasticsearchSearchResponse<T>
{
    public HitsWrapper<T> hits { get; set; }
}

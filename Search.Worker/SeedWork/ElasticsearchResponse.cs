namespace Search.Worker.SeedWork;

public class ElasticsearchResponse<T>
{
    public T _source { get; set; }
}

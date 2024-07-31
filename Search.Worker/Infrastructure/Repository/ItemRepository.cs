using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repository;
using Search.Worker.Exceptions;
using Search.Worker.SeedWork;
using System.Net;
using System.Net.Http.Json; 

namespace Search.Worker.Infrastructure.Repository;

public class ItemRepository(IHttpClientFactory httpClientFactory) : IItemRepository
{

    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    
    private const string _Index = "items"; 

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("Elasticsearch");
    }
    public async Task<bool> CreateItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync($"{_Index}/_doc/{item.Id}", item, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new ConflictException();
        }

        // Don't move this line of code on top of above codes
        response.EnsureSuccessStatusCode();
         
        return true;
    }

    public async Task<Item?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"{_Index}/_doc/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        // Don't move this line of code on top of above codes 
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ElasticsearchResponse<Item>>(cancellationToken);

        return result!._source;
    }

    public async Task<bool> DeleteItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        
        var response = await client.DeleteAsync($"{_Index}/_doc/{item.Id}", cancellationToken); 

        response.EnsureSuccessStatusCode();

        return true;
    }

    public async Task<bool> UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync($"{_Index}/_doc/{item.Id}",item , cancellationToken); 
         
        response.EnsureSuccessStatusCode();

        return true;
    }
}

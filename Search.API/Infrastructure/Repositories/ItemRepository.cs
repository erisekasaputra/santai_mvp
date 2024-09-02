using Elastic.Clients.Elasticsearch; 
using Search.API.Applications.Dto;
using Search.API.Domain.Models;
using Search.API.Domain.Repositories;

namespace Search.API.Infrastructure.Repositories;

public class ItemRepository(ElasticsearchContext context) : IItemRepository
{
    const string _Index = "items";
    private readonly ElasticsearchContext _context = context;

    public async Task<IEnumerable<Item>> GetPaginatedBySearchQueryAsync(SearchRequestDto query)
    { 
        int pageSize = 10;  
        int from = (query.PageNumber - 1) * pageSize; 

        SearchResponse<Item>? response = null;

        if (!string.IsNullOrWhiteSpace(query.BrandId))
        {
            response = await _context.Client.SearchAsync<Item>(s => s
                .Index(_Index)
                .From(from)
                .Size(pageSize)
                .Query(q => q.Bool(b =>
                    b.Must(mu => mu
                        .Match(m => m
                            .Field(f => f.CategoryId)
                            .Query(query.CategoryId)
                        ), mu => mu
                        .Match(m => m
                            .Field(f => f.BrandId)
                            .Query(query.BrandId))
                        )
                    )
                )
                .Sort(so => so
                    .Field(f => f.Name)
                    .Score(sc => sc.Order(SortOrder.Asc)))
            );
        }
        else
        {
            response = await _context.Client.SearchAsync<Item>(s => s
                .Index(_Index)
                .From(from)
                .Size(pageSize)
                .Query(q => q.Bool(b =>
                   b.Must(mu => mu
                       .Match(m => m
                           .Field(f => f.CategoryId)
                           .Query(query.CategoryId))
                       )
                   )
                )
                .Sort(so => so
                    .Field(f => f.Name)
                    .Score(sc => sc.Order(SortOrder.Asc)))
            );
        } 

        if (response is null)
        {
            return [];
        }

        if (response.IsValidResponse)
        {
            return response.Documents; 
        }
         
        return []; 
    }
}

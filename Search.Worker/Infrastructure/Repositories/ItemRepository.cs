using Elastic.Clients.Elasticsearch;
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Infrastructure.Repositories;

public class ItemRepository(ElasticsearchContext context) : IItemRepository
{
    const string _Index = "items";
    private readonly ElasticsearchContext _context = context;

    public async Task<bool> CreateItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var response = await _context.Client.IndexAsync(item, index: _Index, cancellationToken);

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }

    public async Task<Item?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Client.GetAsync<Item>(id, index => index.Index(_Index), cancellationToken);

        if (response.IsValidResponse)
        {
            return response.Source;
        }

        return null;
    }

    public async Task<bool> DeleteItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var response = await _context.Client.DeleteAsync(index: _Index, item.Id, cancellationToken);

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        var response = await _context.Client.UpdateAsync<Item, Item>(index: _Index, item.Id, u => u.Doc(item), cancellationToken);

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateCategoryByCategoryIdAsync(string Id, string Name, string ImageUrl)
    {
        var response = await _context.Client.UpdateByQueryAsync<Item>(u => u
            .Indices(_Index)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.CategoryId!.Suffix("keyword")).Value(Id))
            )
            .Script(s => s
                    .Source("ctx._source.categoryName = params.newCategoryName; ctx._source.categoryImageUrl = params.newCategoryImageUrl")
                    .Params(p => p
                            .Add("newCategoryName", Name)
                            .Add("newCategoryImageUrl", ImageUrl)
                )
            )
        );

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateBrandByBrandIdAsync(string Id, string Name, string ImageUrl)
    {
        var response = await _context.Client.UpdateByQueryAsync<Item>(u => u
            .Indices(_Index)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.BrandId!.Suffix("keyword")).Value(Id))
            )
            .Script(s => s
                    .Source("ctx._source.brandName = params.newBrandName;ctx._source.brandImageUrl=params.newBrandImageUrl")
                    .Params(p => p
                            .Add("newBrandName", Name)
                            .Add("newBrandImageUrl", ImageUrl)
                )
            )
        );

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }


    public async Task<bool> DeleteCategoryByCategoryIdAsync(string Id)
    {
        var response = await _context.Client.UpdateByQueryAsync<Item>(u => u
            .Indices(_Index)
            .Query(q => q
                .Term(t => t
                    .Field(f => f.CategoryId!.Suffix("keyword")).Value(Id))
            )
            .Script(s => s
                    .Source("ctx._source.categoryId = params.newCategoryId; ctx._source.categoryName = params.newCategoryName; ctx._source.categoryImageUrl = params.newCategoryImageUrl")
                    .Params(p => p
                            .Add("newCategoryId", string.Empty)
                            .Add("newCategoryName", string.Empty)
                            .Add("newCategoryImageUrl", string.Empty)
                )
            )
        );

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteBrandByBrandIdAsync(string Id)
    {
        var response = await _context.Client.UpdateByQueryAsync<Item>(u => u
           .Indices(_Index)
           .Query(q => q
               .Term(t => t
                   .Field(f => f.BrandId!.Suffix("keyword")).Value(Id))
           )
           .Script(s => s
                   .Source("ctx._source.brandId = params.newBrandId; ctx._source.brandName = params.newBrandName; ctx._source.brandImageUrl = params.newBrandImageUrl")
                   .Params(p => p
                           .Add("newBrandId", string.Empty)
                           .Add("newBrandName", string.Empty)
                           .Add("newBrandImageUrl", string.Empty)
               )
           )
        );

        if (response.IsValidResponse)
        {
            return true;
        }

        return false;
    }
}

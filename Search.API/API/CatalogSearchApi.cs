using Microsoft.AspNetCore.Mvc;
using Search.API.Applications.Dto;
using Search.API.Domain.Repositories;

namespace Search.API.API;

public static class CatalogSearchApi
{
    public static IEndpointRouteBuilder MapCatalogSearchApi(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("api/v1/search/catalog");

        app.MapPost("/", GetPaginatedCatalog);

        return app;
    }

    private static async Task<IResult> GetPaginatedCatalog(
        [FromBody] SearchRequestDto request,
        [FromServices] IItemRepository itemRepository)
    { 
        return await Task.FromResult(TypedResults.Ok(await itemRepository.GetPaginatedBySearchQueryAsync(request)));
    }
}

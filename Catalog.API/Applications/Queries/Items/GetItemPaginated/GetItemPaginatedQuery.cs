
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemPaginated;

public record GetItemPaginatedQuery(
    int PageNumber = 1, 
    int PageSize = 10,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    bool AvailableStockOnly = true) : IRequest<Result>;

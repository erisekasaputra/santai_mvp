
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemPaginated;

public record GetItemPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result>;

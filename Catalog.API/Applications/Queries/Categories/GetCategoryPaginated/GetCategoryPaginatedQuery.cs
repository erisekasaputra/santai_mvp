
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryPaginated;

public record GetCategoryPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result>;

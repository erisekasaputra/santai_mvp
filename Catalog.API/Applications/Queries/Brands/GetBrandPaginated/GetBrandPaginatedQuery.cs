
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandPaginated;

public record GetBrandPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result>;

using Catalog.API.DTOs.Brand;
using Catalog.API.DTOs.PaginatedResponse;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandPaginated;

public record GetBrandPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResponseDto<BrandDto>>>;

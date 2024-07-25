using Catalog.API.DTOs.BrandDto;
using Catalog.API.DTOs.PaginatedResponseDto;
using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Queries.Brands.GetBrandPaginated;

public record GetBrandPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResponseDto<BrandDto>>>;

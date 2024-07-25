using Catalog.API.DTOs.CategoryDto;
using Catalog.API.DTOs.PaginatedResponseDto;
using Catalog.API.SeedWorks;
using MediatR;

namespace Catalog.API.Queries.Categories.GetCategoryPaginated;

public record GetCategoryPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResponseDto<CategoryDto>>>;

using Catalog.API.DTOs.Category;
using Catalog.API.DTOs.PaginatedResponse;
using Catalog.API.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryPaginated;

public record GetCategoryPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResponseDto<CategoryDto>>>;

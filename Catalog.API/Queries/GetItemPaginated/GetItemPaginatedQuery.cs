using Catalog.API.DTOs.ItemDto;
using Catalog.API.DTOs.PaginatedResponseDto;
using Catalog.API.SeedWorks; 
using MediatR;

namespace Catalog.API.Queries.GetItemPaginated;

public record GetItemPaginatedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResponseDto<ItemDto>>>;

using Catalog.API.DTOs.Item;
using Catalog.API.DTOs.PaginatedResponse;
using Catalog.API.Extensions;
using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemPaginated;

public class GetItemPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetItemPaginatedQuery, Result<PaginatedResponseDto<ItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PaginatedResponseDto<ItemDto>>> Handle(GetItemPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Items) = await _unitOfWork.Items.GetPaginatedItemsAsync(request.PageNumber, request.PageSize);

        if (Items is null)
        {
            return Result<PaginatedResponseDto<ItemDto>>.Failure("Data not found", 404);
        }

        var result = new PaginatedResponseDto<ItemDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Items.ToItemsDto().ToList());

        return Result<PaginatedResponseDto<ItemDto>>.SuccessResult(result, [], 200);
    }
}

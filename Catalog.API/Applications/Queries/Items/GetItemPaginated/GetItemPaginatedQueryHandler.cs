
using Catalog.API.Applications.Dtos.Item;
using Catalog.API.Applications.Dtos.PaginatedResponse;
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemPaginated;

public class GetItemPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetItemPaginatedQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetItemPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Items) = await _unitOfWork.Items.GetPaginatedItemsAsync(request.PageNumber, request.PageSize);

        if (Items is null)
        {
            return Result.Failure("Data not found", ResponseStatus.NotFound);
        }

        var result = new PaginatedResponseDto<ItemDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Items.ToItemsDto().ToList());

        return Result.Success(result, ResponseStatus.Ok);
    }
}

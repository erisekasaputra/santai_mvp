using Catalog.API.DTOs.Item;
using Catalog.API.Extensions;
using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Queries.Items.GetItemById;

public class GetItemByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (result is null)
        {
            return Result<ItemDto>.Failure([$"Product with id {request.Id} not found"], 404);
        }

        return Result<ItemDto>.SuccessResult(result.ToItemDto(), [], 201);
    }
}

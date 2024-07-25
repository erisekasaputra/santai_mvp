using Catalog.API.DTOs.ItemDto; 
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Queries.GetItemById;

public class GetItemByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{ 
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (result == null)
        {
            return Result<ItemDto>.Failure([$"Product with id {request.Id} not found"], 404);
        }

        return Result<ItemDto>.SuccessResult(ItemDto.Create(
                result.Id,
                result.Name,
                result.Description,
                result.Price,
                result.ImageUrl,
                result.CreatedAt,
                result.StockQuantity,
                result.SoldQuantity,
                result.CategoryId,
                result.Category.Name
            ), []);
    }
}

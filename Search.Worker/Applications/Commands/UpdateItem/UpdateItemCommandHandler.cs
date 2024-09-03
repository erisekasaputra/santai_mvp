using MediatR;
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.UpdateItem;

public class UpdateItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<UpdateItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    { 
        var ownerReviews = request.OwnerReviews?.Select(x => new OwnerReview(x.Title, x.Rating));

        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.Update(
            request.Name,
            request.Description,
            request.Sku,
            request.Price,
            request.ImageUrl,
            request.StockQuantity,
            request.SoldQuantity,
            request.CategoryId,
            request.CategoryName,
            request.CategoryImageUrl,
            request.BrandId,
            request.BrandName,
            request.BrandImageUrl,
            request.IsActive,
            request.IsDeleted,
            ownerReviews); 

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

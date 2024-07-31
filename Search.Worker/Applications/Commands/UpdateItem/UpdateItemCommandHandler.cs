using MediatR; 
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repository; 

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

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.ImageUrl = request.ImageUrl;
        item.StockQuantity = request.StockQuantity;
        item.SoldQuantity = request.SoldQuantity;
        item.CategoryId = request.CategoryId;
        item.CategoryName = request.CategoryName;
        item.BrandId = request.BrandId;
        item.BrandName = request.BrandName; 

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

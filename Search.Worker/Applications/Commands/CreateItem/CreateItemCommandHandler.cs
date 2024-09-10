using MediatR;
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.CreateItem;

public class CreateItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<CreateItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var ownerReviews = request.OwnerReviews?.Select(x => new OwnerReview(x.Title, x.Rating));

        var existingItem = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (existingItem is not null)
        {
            return Unit.Value;
        }
        
        var item = new Item(
            request.Id,
            request.Name,
            request.Description,
            request.Sku,
            request.Price,
            request.Currency,
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

        await _itemRepository.CreateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

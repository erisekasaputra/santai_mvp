using MediatR; 
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repository; 

namespace Search.Worker.Applications.Commands.CreateItem;

public class CreateItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<CreateItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var itemExist = _itemRepository.GetItemById(request.Id, cancellationToken);

        if (itemExist is null)
        {
            return await Task.FromResult(Unit.Value);
        }

        var item = new Item(request.Id, request.Name, request.Description, request.Price, request.ImageUrl, request.CreatedAt, request.StockQuantity, request.SoldQuantity, request.CategoryId, request.CategoryName, request.BrandId, request.BrandName);

        await _itemRepository.CreateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

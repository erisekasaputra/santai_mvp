using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.AddItemStockQuantity;

internal class AddItemStockQuantityCommandHandler(IItemRepository itemRepository) : IRequestHandler<AddItemStockQuantityCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(AddItemStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }
         
        item.AddStockQuantity(request.Quantity);

        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

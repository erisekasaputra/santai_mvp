using MediatR;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.ReduceItemStockQuantity;

internal class ReduceItemStockQuantityCommandHandler(IItemRepository itemRepository) : IRequestHandler<ReduceItemStockQuantityCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(ReduceItemStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }
         
        item.ReduceStockQuantity(request.Quantity);

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

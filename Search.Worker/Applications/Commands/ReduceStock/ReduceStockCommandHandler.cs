using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.ReduceStock;

internal class ReduceStockCommandHandler(IItemRepository itemRepository) : IRequestHandler<ReduceStockCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(ReduceStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.StockQuantity -= request.Quantity;

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

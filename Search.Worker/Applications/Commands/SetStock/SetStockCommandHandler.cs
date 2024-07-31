using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.SetStock;

internal class SetStockCommandHandler(IItemRepository itemRepository) : IRequestHandler<SetStockCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(SetStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.StockQuantity = request.Quantity;

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

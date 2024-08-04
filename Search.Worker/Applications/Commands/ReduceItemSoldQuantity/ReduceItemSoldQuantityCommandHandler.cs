using MediatR; 
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.ReduceItemSoldQuantity;

internal class ReduceItemSoldQuantityCommandHandler(IItemRepository itemRepository) : IRequestHandler<ReduceItemSoldQuantityCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(ReduceItemSoldQuantityCommand request, CancellationToken cancellationToken)
    { 
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.ReduceSoldQuantity(request.Quantity); 

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

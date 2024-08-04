using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.SetItemSoldQuantity;

internal class SetItemSoldQuantityCommandHandler(IItemRepository itemRepository) : IRequestHandler<SetItemSoldQuantityCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(SetItemSoldQuantityCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.SetSoldQuantity(request.Quantity);

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

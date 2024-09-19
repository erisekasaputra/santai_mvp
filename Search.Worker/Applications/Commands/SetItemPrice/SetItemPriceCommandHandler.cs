using MediatR;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.SetItemPrice;

internal class SetItemPriceCommandHandler(IItemRepository itemRepository) : IRequestHandler<SetItemPriceCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(SetItemPriceCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        } 

        item.SetItemPrice(request.OldPrice, request.NewPrice, request.Currency);

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

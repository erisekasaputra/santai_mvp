using MediatR;
using Search.Worker.Domain.Repository; 

namespace Search.Worker.Applications.Commands.SetPrice;

internal class SetPriceCommandHandler(IItemRepository itemRepository) : IRequestHandler<SetPriceCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(SetPriceCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.Price = request.Price;

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

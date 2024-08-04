using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.AddItemSoldQuantity;

internal class AddItemSoldQuantityCommandHandler(IItemRepository itemRepository) : IRequestHandler<AddItemSoldQuantityCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(AddItemSoldQuantityCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return await Task.FromResult(Unit.Value);
        }
        
        item.AddSoldQuantity(request.Quantity);

        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

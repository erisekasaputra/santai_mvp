using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.AddSold;

internal class AddSoldCommandHandler(IItemRepository itemRepository) : IRequestHandler<AddSoldCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(AddSoldCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return await Task.FromResult(Unit.Value);
        }
        
        item.SoldQuantity += request.Quantity;

        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

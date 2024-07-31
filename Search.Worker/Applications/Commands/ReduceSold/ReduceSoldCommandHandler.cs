using MediatR;
using Search.Worker.Domain.Models;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.ReduceSold;

internal class ReduceSoldCommandHandler(IItemRepository itemRepository) : IRequestHandler<ReduceSoldCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(ReduceSoldCommand request, CancellationToken cancellationToken)
    { 
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.SoldQuantity -= request.Quantity;

        await _itemRepository.UpdateItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

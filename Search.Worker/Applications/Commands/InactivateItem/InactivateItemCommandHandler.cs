using MediatR;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.InactivateItem;

internal class InactivateItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<InactivateItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(InactivateItemCommand request, CancellationToken cancellationToken)
    { 
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        item.Deactivate();

        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

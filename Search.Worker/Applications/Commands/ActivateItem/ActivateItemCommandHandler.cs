using MediatR;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.ActivateItem;

internal class ActivateItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<ActivateItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(ActivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }

        if (item.IsDeleted)
        {
            return Unit.Value;
        }

        item.Activate();
        
        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

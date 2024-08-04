using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.UndeleteItem;

internal class UndeleteItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<UndeleteItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task<Unit> Handle(UndeleteItemCommand request, CancellationToken cancellationToken)
    {    
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        } 

        item.Undelete();

        await _itemRepository.UpdateItemAsync(item, cancellationToken);

        return Unit.Value;
    }
}

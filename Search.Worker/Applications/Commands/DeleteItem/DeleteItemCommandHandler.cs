using MediatR; 
using Search.Worker.Domain.Repository; 

namespace Search.Worker.Applications.Commands.DeleteItem;

public class DeleteItemCommandHandler(IItemRepository itemRepository) : IRequestHandler<DeleteItemCommand, Unit>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Unit.Value;
        }    

        await _itemRepository.DeleteItemAsync(item, cancellationToken);
        return Unit.Value;
    }
}

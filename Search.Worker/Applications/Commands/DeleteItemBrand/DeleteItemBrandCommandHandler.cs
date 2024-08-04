using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.DeleteItemBrand;

public class DeleteItemBrandCommandHandler(IItemRepository itemRepository) : IRequestHandler<DeleteItemBrandCommand>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task Handle(DeleteItemBrandCommand request, CancellationToken cancellationToken)
    { 
        await _itemRepository.DeleteBrandByBrandIdAsync(request.Id); 
    }
}

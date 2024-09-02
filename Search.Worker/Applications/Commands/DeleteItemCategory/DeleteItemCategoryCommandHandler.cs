using MediatR;
using Search.Worker.Domain.Repositories;

namespace Search.Worker.Applications.Commands.DeleteItemCategory;

public class DeleteItemCategoryCommandHandler(IItemRepository itemRepository) : IRequestHandler<DeleteItemCategoryCommand>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task Handle(DeleteItemCategoryCommand request, CancellationToken cancellationToken)
    {   
        await _itemRepository.DeleteCategoryByCategoryIdAsync(request.Id); 
    }
}

using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.UpdateItemCategory;

public class UpdateItemCategoryCommandHandler(IItemRepository itemRepository) : IRequestHandler<UpdateItemCategoryCommand>
{ 
    private readonly IItemRepository _itemRepository = itemRepository;
    public async Task Handle(UpdateItemCategoryCommand request, CancellationToken cancellationToken)
    {
        await _itemRepository.UpdateCategoryByCategoryIdAsync(request.Id, request.Name, request.ImageUrl);
    }
}

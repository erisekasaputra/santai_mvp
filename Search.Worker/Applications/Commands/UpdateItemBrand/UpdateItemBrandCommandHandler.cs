using MediatR;
using Search.Worker.Domain.Repository;

namespace Search.Worker.Applications.Commands.UpdateItemBrand;

public class UpdateItemBrandCommandHandler(IItemRepository itemRepository) : IRequestHandler<UpdateItemBrandCommand>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public async Task Handle(UpdateItemBrandCommand request, CancellationToken cancellationToken)
    { 
        await _itemRepository.UpdateBrandByBrandIdAsync(request.Id, request.Name, request.ImageUrl);
    }
}

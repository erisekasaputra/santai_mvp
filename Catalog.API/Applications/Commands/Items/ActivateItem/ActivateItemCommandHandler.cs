using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork; 
using MediatR;

namespace Catalog.API.Applications.Commands.Items.ActivateItem;

public class ActivateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ActivateItemCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(ActivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (item is not null)
        { 
            item.SetActive(); 

            _unitOfWork.Items.UpdateItem(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

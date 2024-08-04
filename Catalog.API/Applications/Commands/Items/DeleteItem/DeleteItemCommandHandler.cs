using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeleteItem;

public class DeleteItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteItemCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (item is not null)
        {  
            item.SetDelete();

            _unitOfWork.Items.UpdateItem(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

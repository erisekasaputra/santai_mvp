using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Commands.DeleteItem;

public class DeleteItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteItemCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (item is not null)
        {
            _unitOfWork.Items.DeleteItem(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

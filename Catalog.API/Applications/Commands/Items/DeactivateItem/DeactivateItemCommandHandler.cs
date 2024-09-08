using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeactivateItem;

public class DeactivateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeactivateItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeactivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetItemByIdAsync(request.Id);

        if (item is not null)
        {
            item.SetInactive();

            _unitOfWork.Items.UpdateItem(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(Unit.Value, ResponseStatus.NoContent);
    }
}

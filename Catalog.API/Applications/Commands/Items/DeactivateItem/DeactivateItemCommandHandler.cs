using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Items.DeactivateItem;

public class DeactivateItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeactivateItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeactivateItemCommand request, CancellationToken cancellationToken)
    {
        try
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
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception)
        {
            return Result.Failure(Messages.InternalServerError, ResponseStatus.BadRequest);
        } 
    }
}

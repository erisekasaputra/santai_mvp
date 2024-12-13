using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _unitOfWork.Categories.GetCategoryByIdAsync(request.Id);

            if (item is not null)
            {
                item.Delete();

                _unitOfWork.Categories.UpdateCategory(item);

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

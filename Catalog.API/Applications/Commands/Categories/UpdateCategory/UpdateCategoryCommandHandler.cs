
using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.Id);

            if (category is null)
            {
                return Result.Failure($"Category with id {request.Id} is not found",
                    ResponseStatus.NotFound);
            }

            category.Update(request.Name, request.ImageUrl);

            _unitOfWork.Categories.UpdateCategory(category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

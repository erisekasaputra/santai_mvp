using Catalog.API.Applications.Dtos.Category;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.CreateCategory;

public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(request.Name);

            if (existingCategory is not null)
            {
                return Result.Failure($"Category {request.Name} already registered", ResponseStatus.Conflict);
            }

            var category = new Category(
                   request.Name,
                   request.ImageUrl
               );

            var response = await _unitOfWork.Categories.CreateCategoryAsync(category);

            if (response is null)
            {
                return Result.Failure("We encountered an issue while creating the category. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
            {
                return Result.Failure("We encountered an issue while creating the category. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
            }

            return Result.Success(new CategoryDto(response.Id, response.Name, response.ImageUrl), ResponseStatus.Created); 
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

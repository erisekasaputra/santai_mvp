using Catalog.API.SeedWorks;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Commands.Categories.CreateCategory;

public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(request.Name);

        if (existingCategory is not null)
        {
            return Result<CreateCategoryResponse>.Failure($"Category {request.Name} already registered", 409);
        }

        var category = new Category( 
               request.Name,
               request.ImageUrl
           );

        var response = await _unitOfWork.Categories.CreateCategoryAsync(category);

        if (response is null)
        {
            return Result<CreateCategoryResponse>.Failure("We encountered an issue while creating the category. Please try again later or contact support if the problem persists.", 500);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            return Result<CreateCategoryResponse>.Failure("We encountered an issue while creating the category. Please try again later or contact support if the problem persists.", 500);
        }

        return Result<CreateCategoryResponse>.SuccessResult(
                CreateCategoryResponse.Create(
                        response.Id,
                        response.Name,
                        response.ImageUrl
                    ), [], 201
            );
    }
}

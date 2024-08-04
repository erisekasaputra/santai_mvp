using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.Id);

        if (category is null)
        {
            return Result<Unit>.Failure($"Category with id {request.Id} is not found", 404);
        }

        category.Update(request.Name, request.ImageUrl);

        _unitOfWork.Categories.UpdateCategory(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

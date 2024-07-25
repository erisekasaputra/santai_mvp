using Catalog.API.DTOs.CategoryDto;
using Catalog.API.Extensions;
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Queries.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Categories.GetCategoryByIdAsync(request.Id);

        if (result is null)
        {
            return Result<CategoryDto>.Failure([$"Category with id {request.Id} not found"], 404);
        }

        return Result<CategoryDto>.SuccessResult(result.ToCategoryDto(), []);
    }
}

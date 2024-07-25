using Catalog.API.DTOs.CategoryDto;
using Catalog.API.DTOs.PaginatedResponseDto;
using Catalog.API.Extensions;
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Queries.Categories.GetCategoryPaginated;

public class GetCategoryPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryPaginatedQuery, Result<PaginatedResponseDto<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PaginatedResponseDto<CategoryDto>>> Handle(GetCategoryPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Categories) = await _unitOfWork.Categories.GetPaginatedCategoriesAsync(request.PageNumber, request.PageSize);

        if (Categories is null)
        {
            return Result<PaginatedResponseDto<CategoryDto>>.Failure("Data not found", 404);
        }

        var result = new PaginatedResponseDto<CategoryDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Categories.ToCategoriesDto().ToList());

        return Result<PaginatedResponseDto<CategoryDto>>.SuccessResult(result, [], 200);
    }
}

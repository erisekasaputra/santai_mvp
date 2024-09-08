
using Catalog.API.Applications.Dtos.Category;
using Catalog.API.Applications.Dtos.PaginatedResponse;
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Categories.GetCategoryPaginated;

public class GetCategoryPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryPaginatedQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetCategoryPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Categories) = await _unitOfWork.Categories.GetPaginatedCategoriesAsync(request.PageNumber, request.PageSize);

        if (Categories is null)
        {
            return Result.Failure("Data not found", ResponseStatus.NotFound);
        }

        var result = new PaginatedResponseDto<CategoryDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Categories.ToCategoriesDto().ToList());

        return Result.Success(result, ResponseStatus.Ok);
    }
}

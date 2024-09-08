
using Catalog.API.Applications.Dtos.Brand;
using Catalog.API.Applications.Dtos.PaginatedResponse;
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandPaginated;

public class GetBrandPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBrandPaginatedQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetBrandPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Brands) = await _unitOfWork.Brands.GetPaginatedBrandsAsync(request.PageNumber, request.PageSize);

        if (Brands is null)
        {
            return Result.Failure("Data not found", ResponseStatus.NotFound);
        }

        var result = new PaginatedResponseDto<BrandDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Brands.ToBrandsDto().ToList());

        return Result.Success(result, ResponseStatus.Ok);
    }
}

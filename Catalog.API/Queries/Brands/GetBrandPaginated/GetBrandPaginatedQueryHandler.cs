using Catalog.API.DTOs.BrandDto;
using Catalog.API.DTOs.PaginatedResponseDto;
using Catalog.API.Extensions;
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Queries.Brands.GetBrandPaginated;

public class GetBrandPaginatedQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBrandPaginatedQuery, Result<PaginatedResponseDto<BrandDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PaginatedResponseDto<BrandDto>>> Handle(GetBrandPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (TotalCount, TotalPages, Brands) = await _unitOfWork.Brands.GetPaginatedBrandsAsync(request.PageNumber, request.PageSize);

        if (Brands is null)
        {
            return Result<PaginatedResponseDto<BrandDto>>.Failure("Data not found", 404);
        }

        var result = new PaginatedResponseDto<BrandDto>(request.PageNumber, request.PageSize, TotalCount, TotalPages, Brands.ToBrandsDto().ToList());

        return Result<PaginatedResponseDto<BrandDto>>.SuccessResult(result, [], 200);
    }
}

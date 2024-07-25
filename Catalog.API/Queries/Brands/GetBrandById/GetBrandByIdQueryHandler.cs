using Catalog.API.DTOs.BrandDto;
using Catalog.API.Extensions;
using Catalog.API.SeedWorks;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Queries.Brands.GetBrandById;

public class GetBrandByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

        if (brand is null)
        {
            return Result<BrandDto>.Failure($"Brand with id {request.Id} is not found", 404);
        }

        return Result<BrandDto>.SuccessResult(brand.ToBrandDto(), []);

    }
}

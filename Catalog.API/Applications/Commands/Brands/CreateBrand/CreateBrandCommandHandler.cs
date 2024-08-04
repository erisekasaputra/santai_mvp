using Catalog.API.DTOs.Brand;
using Catalog.API.SeedWork;
using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.CreateBrand;

public class CreateBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateBrandCommand, Result<BrandDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<BrandDto>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var existingBrand = await _unitOfWork.Brands.GetBrandByNameAsync(request.Name);

        if (existingBrand is not null)
        {
            return Result<BrandDto>.Failure($"Brand {request.Name} already registered", 409);
        }

        var brand = new Brand(
               request.Name,
               request.ImageUrl 
           );

        var response = await _unitOfWork.Brands.CreateBrandAsync(brand);

        if (response is null)
        {
            return Result<BrandDto>.Failure("We encountered an issue while creating the brand. Please try again later or contact support if the problem persists.", 500);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            return Result<BrandDto>.Failure("We encountered an issue while creating the brand. Please try again later or contact support if the problem persists.", 500);
        }

        return Result<BrandDto>.SuccessResult(new BrandDto(response.Id, response.Name, response.ImageUrl), [], 201);
    }
}

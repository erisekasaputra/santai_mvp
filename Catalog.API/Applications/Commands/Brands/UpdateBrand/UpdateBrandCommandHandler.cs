using Catalog.API.SeedWork;
using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.UpdateBrand;

public class UpdateBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateBrandCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<Unit>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

        if (brand is null)
        {
            return Result<Unit>.Failure($"Brand with id {request.Id} is not found", 404);
        }

        brand.Update(request.Name, request.ImageUrl); 

        _unitOfWork.Brands.UpdateBrand(brand);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.SuccessResult(Unit.Value, [], 204);
    }
}

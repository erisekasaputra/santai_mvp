 
using Catalog.API.Extensions; 
using Catalog.Domain.SeedWork;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Queries.Brands.GetBrandById;

public class GetBrandByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBrandByIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

        if (brand is null)
        {
            return Result.Failure($"Brand with id {request.Id} is not found", ResponseStatus.NotFound);
        }

        return Result.Success(brand.ToBrandDto(), ResponseStatus.Ok);

    }
}

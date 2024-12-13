using Catalog.API.Applications.Dtos.Brand;
using Catalog.Domain.Aggregates.BrandAggregate;
using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.CreateBrand;

public class CreateBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateBrandCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingBrand = await _unitOfWork.Brands.GetBrandByNameAsync(request.Name);

            if (existingBrand is not null)
            {
                return Result.Failure($"Brand {request.Name} already registered", ResponseStatus.Conflict);
            }

            var brand = new Brand(
                   request.Name,
                   request.ImageUrl
               );

            var response = await _unitOfWork.Brands.CreateBrandAsync(brand);

            if (response is null)
            {
                return Result.Failure("We encountered an issue while creating the brand. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
            {
                return Result.Failure("We encountered an issue while creating the brand. Please try again later or contact support if the problem persists.", ResponseStatus.InternalServerError);
            }

            return Result.Success(new BrandDto(response.Id, response.Name, response.ImageUrl), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception)
        {
            return Result.Failure(Messages.InternalServerError, ResponseStatus.BadRequest);
        } 
    }
}

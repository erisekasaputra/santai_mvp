using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.UpdateBrand;

public class UpdateBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

            if (brand is null)
            {
                return Result.Failure($"Brand with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            brand.Update(request.Name, request.ImageUrl);

            _unitOfWork.Brands.UpdateBrand(brand);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(Unit.Value, ResponseStatus.NoContent);
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

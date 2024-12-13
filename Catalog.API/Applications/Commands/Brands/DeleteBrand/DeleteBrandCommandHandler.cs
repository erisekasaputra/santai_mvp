using Catalog.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using MediatR;

namespace Catalog.API.Applications.Commands.Brands.DeleteBrand;

public class DeleteBrandCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {  
            var item = await _unitOfWork.Brands.GetBrandByIdAsync(request.Id);

            if (item is not null)
            {
                item.Delete();

                _unitOfWork.Brands.UpdateBrand(item);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

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

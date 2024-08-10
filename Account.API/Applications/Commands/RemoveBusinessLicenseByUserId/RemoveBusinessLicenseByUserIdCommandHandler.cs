using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RemoveBusinessLicenseByUserId;

public class RemoveBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<RemoveBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(RemoveBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var license = await _unitOfWork.BusinessLicenses.GetByBusinessUserIdAndBusinessLicenseIdAsync(request.BusinessUserId, request.BusinessLicenseId);

            if (license is null)
            {
                return Result.Failure($"Business license '{request.BusinessLicenseId}' with related business user '{request.BusinessUserId}' not found", ResponseStatus.NotFound);
            } 

            _unitOfWork.BusinessLicenses.Delete(license);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

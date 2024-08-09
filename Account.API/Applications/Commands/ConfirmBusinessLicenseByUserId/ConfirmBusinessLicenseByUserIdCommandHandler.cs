using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork; 
using MediatR;

namespace Account.API.Applications.Commands.ConfirmBusinessLicenseByUserId;

public class ConfirmBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service): IRequestHandler<ConfirmBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(ConfirmBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.BusinessUserId);

            if (user is null)
            {
                return Result.Failure($"Business user '{request.BusinessUserId}' not found", ResponseStatus.NotFound);
            }

            var license = await _unitOfWork.BusinessLicenses.GetByIdAsync(request.BusinessLicenseId);

            if (license is null)
            {
                return Result.Failure($"Business license '{request.BusinessLicenseId}' not found", ResponseStatus.NotFound);
            }

            var acceptedLicense = await _unitOfWork.BusinessLicenses.GetAcceptedByNumberAsNoTrackAsync(license.LicenseNumber);

            if (acceptedLicense is not null && acceptedLicense.Id == request.BusinessLicenseId)
            {
                return Result.Failure($"Business license '{acceptedLicense.Id}' already confirmed", ResponseStatus.NoContent);
            }

            if (acceptedLicense is not null)
            {  
                return Result.Failure($"Can not have multiple license number '{license.LicenseNumber}' with 'Accepted' status", ResponseStatus.Conflict);
            } 

            license.VerifyDocument();

            _unitOfWork.BusinessLicenses.Update(license);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        { 
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

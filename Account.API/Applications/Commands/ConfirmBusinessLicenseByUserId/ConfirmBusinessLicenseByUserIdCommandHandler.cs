using Account.API.Applications.Services;
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
                return Result.Failure($"Business user with id {request.BusinessUserId} is not found", ResponseStatus.NotFound);
            }

            var license = await _unitOfWork.BusinessLicenses.GetByIdAsync(request.BusinessLicenseId);

            if (license is null)
            {
                return Result.Failure($"Business license with id {request.BusinessLicenseId} is not found", ResponseStatus.NotFound);
            }

            var acceptedLicense = await _unitOfWork.BusinessLicenses.GetAcceptedByNumberAsNoTrackAsync(license.LicenseNumber);

            if (acceptedLicense is not null)
            {
                return Result.Failure($"Failed to verify the business license because there is already a business license with the status 'Accepted' and the same license number '{license.LicenseNumber}'", ResponseStatus.Conflict);
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
            return Result.Failure("An error has occurred while rejecting business license", ResponseStatus.InternalServerError);
        }
    }
}

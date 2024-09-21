using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.BusinessUserCommand.ConfirmBusinessLicenseByUserId;

public class ConfirmBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<ConfirmBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(ConfirmBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var license = await _unitOfWork.BusinessLicenses.GetByBusinessUserIdAndBusinessLicenseIdAsync(request.BusinessUserId, request.BusinessLicenseId);

            if (license is null)
            {
                return Result.Failure($"Business license not found", ResponseStatus.NotFound)
                    .WithError(new ("BusinessLicense.LicenseNumber", "Business license not found"));
            }

            var acceptedLicense = await _unitOfWork.BusinessLicenses.GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(license.HashedLicenseNumber);

            if (acceptedLicense is not null && acceptedLicense.Id == request.BusinessLicenseId)
            {
                return Result.Failure($"Business license '{acceptedLicense.Id}' already confirmed", ResponseStatus.Conflict)
                    .WithError(new ("BusinessLicense.LicenseNumber", "Business license is confirmed"));
            }

            if (acceptedLicense is not null)
            {
                return Result.Failure($"Conflict business license", ResponseStatus.Conflict)
                    .WithError(new("BusinessLicense.LicenseNumber", "Can not have multiple license number with 'Accepted' status"));
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
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

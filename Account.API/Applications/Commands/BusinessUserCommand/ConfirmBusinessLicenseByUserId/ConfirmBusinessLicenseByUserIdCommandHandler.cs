using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

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
                return Result.Failure($"Busines user id '{request.BusinessUserId}' and business license '{request.BusinessLicenseId}' did not match any record", ResponseStatus.NotFound);
            }

            var acceptedLicense = await _unitOfWork.BusinessLicenses.GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(license.HashedLicenseNumber);

            if (acceptedLicense is not null && acceptedLicense.Id == request.BusinessLicenseId)
            {
                return Result.Failure($"Business license '{acceptedLicense.Id}' already confirmed", ResponseStatus.Conflict);
            }

            if (acceptedLicense is not null)
            {
                return Result.Failure($"Can not have multiple license number with 'Accepted' status", ResponseStatus.Conflict);
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
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

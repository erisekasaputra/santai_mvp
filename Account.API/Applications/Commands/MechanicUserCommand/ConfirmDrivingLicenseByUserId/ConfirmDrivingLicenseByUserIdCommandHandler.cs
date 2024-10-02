using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR; 
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmDrivingLicenseByUserId;

public class ConfirmDrivingLicenseByUserIdCommandHandler : IRequestHandler<ConfirmDrivingLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service; 

    public ConfirmDrivingLicenseByUserIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ApplicationService service )
    {
        _unitOfWork = unitOfWork; 
        _service = service; 
    }

    public async Task<Result> Handle(ConfirmDrivingLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var license = await _unitOfWork.DrivingLicenses.GetByUserIdAndIdAsync(request.UserId, request.DrivingLicenseId);

            if (license is null)
            {
                return Result.Failure($"Driving license not found", ResponseStatus.NotFound);
            }
             
            var accepted = await _unitOfWork.DrivingLicenses.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.Id == request.DrivingLicenseId)
            {
                return Result.Failure($"Driving license already accepted", ResponseStatus.Conflict);
            }

            if (accepted is not null)
            {
                return Result.Failure($"Can only have one 'Accepted' driving license for a user", ResponseStatus.Conflict);
            }

            license.VerifyDocument();

            _unitOfWork.DrivingLicenses.Update(license);

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

using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectDrivingLicenseByUserId;

public class RejectDrivingLicenseByUserIdCommandHandler : IRequestHandler<RejectDrivingLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public RejectDrivingLicenseByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<ReferralProgramOption> referralOptions,
        ApplicationService service,
        IKeyManagementService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _referralOptions = referralOptions;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(RejectDrivingLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var drivingLicense = await _unitOfWork.DrivingLicenses.GetByUserIdAndIdAsync(request.UserId, request.DrivingLicenseId);

            if (drivingLicense is null)
            {
                return Result.Failure($"Driving license not found", ResponseStatus.NotFound)
                     .WithError(new("DrivingLicense.Id", "Driving license not found"));
            }

            drivingLicense.RejectDocument();

            _unitOfWork.DrivingLicenses.Update(drivingLicense);

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

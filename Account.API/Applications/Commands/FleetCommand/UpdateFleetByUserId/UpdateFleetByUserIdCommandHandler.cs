using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services; 
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;

namespace Account.API.Applications.Commands.FleetCommand.UpdateFleetByUserId;

public class UpdateFleetByUserIdCommandHandler : IRequestHandler<UpdateFleetByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public UpdateFleetByUserIdCommandHandler(
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

    public async Task<Result> Handle(UpdateFleetByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var errors = new List<ErrorDetail>();

            var encryptedRegistrationNumber = await EncryptAsync(request.RegistrationNumber);
            var encryptedEngineNumber = await EncryptAsync(request.EngineNumber);
            var encryptedChassisNumber = await EncryptAsync(request.ChassisNumber);
            var encryptedInsuranceNumber = await EncryptAsync(request.InsuranceNumber);
            var encryptedOwnerName = await EncryptAsync(request.OwnerName);
            var encryptedOwnerAddress = await EncryptAsync(request.OwnerAddress);

            var hashedRegistrationNumber = await HashAsync(request.RegistrationNumber);
            var hashedEngineNumber = await HashAsync(request.EngineNumber);
            var hashedChassisNumber = await HashAsync(request.ChassisNumber);
            var hashedInsuranceNumber = await HashAsync(request.InsuranceNumber);

            var timeZoneId = await _unitOfWork.Users.GetTimeZoneById(request.UserId);

            if (timeZoneId is null)
            {
                return Result.Failure("User not found", ResponseStatus.NotFound);
            }
              

            var conflict = await _unitOfWork.Fleets.GetByIdentityExcludingUserIdAsync(
                request.UserId,
                (FleetLegalParameter.EngineNumber, hashedEngineNumber),
                (FleetLegalParameter.ChassisNumber, hashedChassisNumber),
                (FleetLegalParameter.RegistrationNumber, hashedRegistrationNumber));

            if (conflict is not null)
            {
                if (conflict.HashedChassisNumber == hashedChassisNumber)
                {
                    errors.Add(new("Fleet.ChassisNumber", "Chassis number already registered"));
                }

                if (conflict.HashedEngineNumber == hashedEngineNumber)
                {
                    errors.Add(new("Fleet.EngineNumber", "Engine number already registered"));
                }

                if (conflict.HashedRegistrationNumber == hashedRegistrationNumber)
                {
                    errors.Add(new("Fleet.RegistrationNumber", "Registration number already registered"));
                }
            }

            if (errors.Count > 0)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure($"There {(errors.Count <= 1 ? "is" : "are")} error(s) need to be fixed",
                    ResponseStatus.BadRequest).WithErrors(errors), cancellationToken);
            } 

            var fleet = await _unitOfWork.Fleets.GetByUserIdAndIdAsync(request.UserId, request.FleetId);

            if (fleet is null)
            {
                return await RollbackAndReturnFailureAsync(
                  Result.Failure($"Fleet not found", ResponseStatus.NotFound), cancellationToken);
            }
             
            fleet.Update(
                hashedRegistrationNumber,
                encryptedRegistrationNumber,
                request.VehicleType,
                request.Make,
                request.Model,
                request.YearOfManufacture,
                hashedChassisNumber,
                encryptedChassisNumber,
                hashedEngineNumber,
                encryptedEngineNumber,
                hashedInsuranceNumber,
                encryptedInsuranceNumber,
                request.IsInsuranceValid,
                request.LastInspectionDateLocal.FromLocalToUtc(timeZoneId),
                request.OdometerReading,
                request.FuelType,
                request.Color,
                encryptedOwnerName,
                encryptedOwnerAddress,
                request.UsageStatus,
                request.OwnershipStatus,
                request.TransmissionType,
                request.ImageUrl);  

            var response = new FleetResponseDto(
                fleet.Id,
                request.RegistrationNumber,
                request.VehicleType,
                request.Make,
                request.Model,
                request.YearOfManufacture,
                request.ChassisNumber,
                request.EngineNumber,
                request.InsuranceNumber,
                request.IsInsuranceValid,
                request.LastInspectionDateLocal,
                request.OdometerReading,
                request.FuelType,
                request.Color,
                request.OwnerName,
                request.OwnerAddress,
                request.UsageStatus,
                request.OwnershipStatus,
                request.TransmissionType,
                request.ImageUrl);

            _unitOfWork.Fleets.Update(fleet);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(
                Result.Failure(ex.Message, ResponseStatus.BadRequest),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError),
                cancellationToken);
        }
    }

    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);

        return result;
    }

    private async Task<string?> EncryptNullableAsync(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            return null;

        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> EncryptAsync(string plaintext)
    {
        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> HashAsync(string plainText)
    {
        return await _hashService.Hash(plainText);
    }
}

using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;
using Core.Configurations;
using Core.Extensions;
using Core.Enumerations;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.FleetCommand.CreateFleetByUserId;

public class CreateFleetByUserIdCommandHandler : IRequestHandler<CreateFleetByUserIdCommand, Result>
{ 
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public CreateFleetByUserIdCommandHandler(
      IUnitOfWork unitOfWork,
      IOptionsMonitor<ReferralProgramConfiguration> referralOptions,
      ApplicationService service,
      IEncryptionService kmsClient,
      IHashService hashService)
    {
        _unitOfWork = unitOfWork; 
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(CreateFleetByUserIdCommand request, CancellationToken cancellationToken)
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

            var timeZoneId = await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId);

            if (timeZoneId is null)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure("User not found", ResponseStatus.NotFound), cancellationToken);
            }

            var conflict = await _unitOfWork.Fleets.GetByIdentityAsync(
                    (FleetLegalParameter.EngineNumber, hashedEngineNumber),
                    (FleetLegalParameter.ChassisNumber, hashedChassisNumber),
                    (FleetLegalParameter.RegistrationNumber, hashedRegistrationNumber));

            if (conflict is not null)
            {
                if (conflict.HashedChassisNumber == hashedChassisNumber)
                {
                    errors.Add(new("ChassisNumber", "Chassis number already registered", request.ChassisNumber, "ChassisNumberValidator", "Error"));
                }

                if (conflict.HashedEngineNumber == hashedEngineNumber)
                {
                    errors.Add(new("EngineNumber", "Engine number already registered", request.EngineNumber, "EngineNumberValidator", "Error"));
                }

                if (conflict.HashedRegistrationNumber == hashedRegistrationNumber)
                {
                    errors.Add(new("RegistrationNumber", "Registration number already registered", request.RegistrationNumber, "RegistrationNumberValidator", "Error"));
                }
            }

            if (errors.Count > 0)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure($"There {(errors.Count <= 1 ? "is" : "are")} error(s) need to be fixed",
                    ResponseStatus.BadRequest).WithErrors(errors), cancellationToken);
            }

            var fleet = new Fleet(
                request.UserId,
                hashedRegistrationNumber,
                encryptedRegistrationNumber,
                request.VehicleType,
                request.Brand,
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
                encryptedOwnerName,
                encryptedOwnerAddress,
                request.UsageStatus,
                request.OwnershipStatus,
                request.TransmissionType,
                request.ImageUrl);


            var userType = await _unitOfWork.BaseUsers.GetUserTypeById(request.UserId);

            if (userType is null)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure("User not found", ResponseStatus.NotFound), cancellationToken);
            }

            if (userType == UserType.MechanicUser)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure("Can not create fleet to mechanic user", ResponseStatus.BadRequest), cancellationToken);
            }


            var response = new FleetResponseDto(
                fleet.Id,
                request.RegistrationNumber,
                request.VehicleType,
                request.Brand,
                request.Model,
                request.YearOfManufacture,
                request.ChassisNumber,
                request.EngineNumber,
                request.InsuranceNumber,
                request.IsInsuranceValid,
                request.LastInspectionDateLocal,
                request.OdometerReading,
                request.FuelType, 
                request.OwnerName,
                request.OwnerAddress,
                request.UsageStatus,
                request.OwnershipStatus,
                request.TransmissionType,
                request.ImageUrl);

            await _unitOfWork.Fleets.CreateAsync(fleet);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(response, ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(
                Result.Failure(ex.Message, ResponseStatus.BadRequest),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex, ex.InnerException?.Message);
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
    private async Task<string> EncryptAsync(string plaintext)
    {
        return await _kmsClient.EncryptAsync(plaintext);
    }

    private async Task<string> HashAsync(string plainText)
    {
        return await _hashService.Hash(plainText);
    }
}

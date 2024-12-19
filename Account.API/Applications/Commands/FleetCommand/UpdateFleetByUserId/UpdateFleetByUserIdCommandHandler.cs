using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;
using Core.Extensions;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.FleetCommand.UpdateFleetByUserId;

public class UpdateFleetByUserIdCommandHandler : IRequestHandler<UpdateFleetByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public UpdateFleetByUserIdCommandHandler(
      IUnitOfWork unitOfWork, 
      ApplicationService service,
      IEncryptionService kmsClient,
      IHashService hashService)
    {
        _unitOfWork = unitOfWork; 
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

            var encryptedRegistrationNumber = string.IsNullOrEmpty(request.RegistrationNumber) ? null : await EncryptAsync(request.RegistrationNumber);
            var encryptedEngineNumber = string.IsNullOrEmpty(request.EngineNumber) ? null : await EncryptAsync(request.EngineNumber);
            var encryptedChassisNumber = string.IsNullOrEmpty(request.ChassisNumber) ? null : await EncryptAsync(request.ChassisNumber);
            var encryptedInsuranceNumber = string.IsNullOrEmpty(request.InsuranceNumber) ? null : await EncryptAsync(request.InsuranceNumber);
            var encryptedOwnerName = string.IsNullOrEmpty(request.OwnerName) ? null : await EncryptAsync(request.OwnerName);
            var encryptedOwnerAddress = string.IsNullOrEmpty(request.OwnerAddress) ? null : await EncryptAsync(request.OwnerAddress);

            var hashedRegistrationNumber = string.IsNullOrEmpty(request.RegistrationNumber) ? null : await HashAsync(request.RegistrationNumber);
            var hashedEngineNumber = string.IsNullOrEmpty(request.EngineNumber) ? null : await HashAsync(request.EngineNumber);
            var hashedChassisNumber = string.IsNullOrEmpty(request.ChassisNumber) ? null : await HashAsync(request.ChassisNumber);
            var hashedInsuranceNumber = string.IsNullOrEmpty(request.InsuranceNumber) ? null : await HashAsync(request.InsuranceNumber);

            var timeZoneId = await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId);

            if (timeZoneId is null)
            {
                return await RollbackAndReturnFailureAsync(Result.Failure("User not found", ResponseStatus.NotFound), cancellationToken);
            }


            var clauses = new List<(FleetLegalParameter parameter, string hashedValue)>();

            if (hashedEngineNumber != null)
            {
                clauses.Add((FleetLegalParameter.EngineNumber, hashedEngineNumber));
            }

            if (hashedChassisNumber != null)
            {
                clauses.Add((FleetLegalParameter.ChassisNumber, hashedChassisNumber));
            }

            if (hashedRegistrationNumber != null)
            {
                clauses.Add((FleetLegalParameter.RegistrationNumber, hashedRegistrationNumber));
            }


            var conflict = await _unitOfWork.Fleets.GetByIdentityExcludingUserIdAsync(
                request.UserId,
                [.. clauses]);

            if (conflict is not null)
            {
                if (!string.IsNullOrEmpty(request.ChassisNumber) && conflict.HashedChassisNumber == hashedChassisNumber)
                {
                    errors.Add(new("ChassisNumber", "Chassis number already registered", request.ChassisNumber, "ChassisNumberValidator", "Error"));
                }

                if (!string.IsNullOrEmpty(request.EngineNumber) && conflict.HashedEngineNumber == hashedEngineNumber)
                {
                    errors.Add(new("EngineNumber", "Engine number already registered", request.EngineNumber, "EngineNumberValidator", "Error"));
                }

                if (!string.IsNullOrEmpty(request.RegistrationNumber) && conflict.HashedRegistrationNumber == hashedRegistrationNumber)
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

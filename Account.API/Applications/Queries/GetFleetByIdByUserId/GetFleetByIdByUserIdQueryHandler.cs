using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Extensions;
using Core.Services.Interfaces;
using Core.CustomMessages;
using Core.Enumerations;

namespace Account.API.Applications.Queries.GetFleetByIdByUserId;

public class GetFleetByIdByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetFleetByIdByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ApplicationService _appService = service;
    public async Task<Result> Handle(GetFleetByIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {

            string? timeZone = null;

            timeZone = await _unitOfWork.Staffs.GetTimeZoneByIdAsync(request.UserId);

            timeZone ??= await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId);

            if (timeZone == null)
            {
                return Result.Failure($"User time zone configuration is not found", ResponseStatus.NotFound);
            }

            var fleet = await _unitOfWork.Fleets.GetByUserIdAndIdAsync(request.UserId, request.FleetId);

            if (fleet is null)
            {
                return Result.Failure($"Fleet not found", ResponseStatus.NotFound);
            }

            var decryptedRegistrationNumber = await DecryptNullableAsync(fleet.EncryptedRegistrationNumber);
            var decryptedEngineNumber = await DecryptNullableAsync(fleet.EncryptedEngineNumber);
            var decryptedChassisNumber = await DecryptNullableAsync(fleet.EncryptedChassisNumber);
            var decryptedInsuranceNumber = await DecryptNullableAsync(fleet.EncryptedInsuranceNumber);
            var decryptedOwnerName = await DecryptNullableAsync(fleet.Owner?.EncryptedOwnerName);
            var decryptedOwnerAddress = await DecryptNullableAsync(fleet.Owner?.EncryptedOwnerAddress);

            var fleetDto = new FleetResponseDto(
                fleet.Id,
                decryptedRegistrationNumber,
                fleet.VehicleType,
                fleet.Brand,
                fleet.Model,
                fleet.YearOfManufacture,
                decryptedChassisNumber,
                decryptedEngineNumber,
                decryptedInsuranceNumber,
                fleet.IsInsuranceValid,
                fleet.LastInspectionDateUtc.FromUtcToLocal(timeZone),
                fleet.OdometerReading,
                fleet.FuelType, 
                decryptedOwnerName,
                decryptedOwnerAddress,
                fleet.UsageStatus,
                fleet.OwnershipStatus,
                fleet.TransmissionType,
                fleet.ImageUrl); 

            return Result.Success(fleetDto);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<string> DecryptAsync(string cipherText)
    {
        return await _kmsClient.DecryptAsync(cipherText);
    }

    private async Task<string?> DecryptNullableAsync(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}

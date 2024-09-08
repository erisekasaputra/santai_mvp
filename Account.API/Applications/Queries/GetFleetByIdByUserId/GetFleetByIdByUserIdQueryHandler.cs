using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages; 
using Account.Domain.SeedWork;
using MediatR;
using Core.Extensions;

namespace Account.API.Applications.Queries.GetFleetByIdByUserId;

public class GetFleetByIdByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    IKeyManagementService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetFleetByIdByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ApplicationService _appService = service;
    public async Task<Result> Handle(GetFleetByIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeZone = await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId);

            if (timeZone == null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }

            var fleet = await _unitOfWork.Fleets.GetByUserIdAndIdAsync(request.UserId, request.FleetId);

            if (fleet is null)
            {
                return Result.Failure($"Fleet not found", ResponseStatus.NotFound);
            }

            var decryptedRegistrationNumber = await DecryptAsync(fleet.EncryptedRegistrationNumber);
            var decryptedEngineNumber = await DecryptAsync(fleet.EncryptedEngineNumber);
            var decryptedChassisNumber = await DecryptAsync(fleet.EncryptedChassisNumber);
            var decryptedInsuranceNumber = await DecryptAsync(fleet.EncryptedInsuranceNumber);
            var decryptedOwnerName = await DecryptAsync(fleet.Owner.EncryptedOwnerName);
            var decryptedOwnerAddress = await DecryptAsync(fleet.Owner.EncryptedOwnerAddress);

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
        if (string.IsNullOrWhiteSpace(cipherText)) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}

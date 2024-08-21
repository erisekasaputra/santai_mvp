using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Infrastructures;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedFleetByUserId;

public class GetPaginatedFleetByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    IKeyManagementService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetPaginatedFleetByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ApplicationService _appService = service;
    public async Task<Result> Handle(GetPaginatedFleetByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeZone = await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId);

            if (timeZone == null) 
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }

            (int totalCount, int totalPages, IEnumerable<Fleet> fleets) = await _unitOfWork.Fleets.GetPaginatedFleetByUserId(
                request.UserId,
                request.PageNumber,
                request.PageSize);

            if (fleets is null)
            {
                return Result.Failure($"Fleet not found", ResponseStatus.NotFound);
            }


            var fleetDtos = new List<FleetResponseDto>();

            foreach (var fleet in fleets)
            {
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
                    fleet.Make,
                    fleet.Model,
                    fleet.YearOfManufacture,
                    decryptedChassisNumber,
                    decryptedEngineNumber,
                    decryptedInsuranceNumber,
                    fleet.IsInsuranceValid,
                    fleet.LastInspectionDateUtc.FromUtcToLocal(timeZone),
                    fleet.OdometerReading,
                    fleet.FuelType,
                    fleet.Color,
                    decryptedOwnerName,
                    decryptedOwnerAddress,
                    fleet.UsageStatus,
                    fleet.OwnershipStatus,
                    fleet.TransmissionType,
                    fleet.ImageUrl); 

                fleetDtos.Add(fleetDto);
            }

            var paginatedResponse = new PaginatedResponseDto<FleetResponseDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages,
                fleetDtos);

            return Result.Success(paginatedResponse);
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
 
using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.SeedWork;
using MediatR;
using Core.Extensions;
using Core.Dtos;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetPaginatedFleetByUserId;

public class GetPaginatedFleetByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetPaginatedFleetByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ApplicationService _appService = service;
    public async Task<Result> Handle(GetPaginatedFleetByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeZone = await _unitOfWork.BaseUsers.GetTimeZoneById(request.UserId); 

            if (timeZone == null) 
            {
                timeZone = await _unitOfWork.Staffs.GetTimeZoneByIdAsync(request.UserId);
                if (timeZone == null)
                {
                    return Result.Failure($"User not found", ResponseStatus.NotFound);
                }
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
        if (string.IsNullOrEmpty(cipherText)) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}
 
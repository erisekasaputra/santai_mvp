using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.Domain.Aggregates.FleetAggregate; 
using Account.Domain.SeedWork;
using Core.Configurations; 
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Options; 

namespace Account.API.Applications.Queries.GetUserByUserTypeAndUserId;

public class GetUserByUserTypeAndUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetUserByUserTypeAndUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;

    public async Task<Result> Handle(GetUserByUserTypeAndUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var baseUser = await _unitOfWork.BaseUsers.GetByIdAsync(request.UserId); 
            if (baseUser is not null)
            {
                var decryptedEmail = await DecryptNullableAsync(baseUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(baseUser.EncryptedPhoneNumber);

                var fleets = await _unitOfWork.Fleets.GetByUserIdAsync(request.UserId);

                (var filteredFleets, var fleetsUnknown) = await GetFleetsDtos(fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    baseUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    baseUser.TimeZoneId,
                    baseUser.Name,
                    filteredFleets,
                    fleetsUnknown,
                    baseUser.DeviceIds);

                return Result.Success(userResponseDto, ResponseStatus.Ok);
            }
             
             
            var staffUser = await _unitOfWork.Staffs.GetByIdAsync(request.UserId);
            if (staffUser is not null)
            {
                var decryptedEmail = await DecryptNullableAsync(staffUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(staffUser.EncryptedPhoneNumber); 

                (var filteredFleets, var fleetsUnknown) = await GetFleetsDtos(staffUser.Fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    staffUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    staffUser.TimeZoneId,
                    staffUser.Name,
                    filteredFleets,
                    fleetsUnknown,
                    staffUser.DeviceIds);

                return Result.Success(userResponseDto, ResponseStatus.Ok);
            }

            return Result.Failure("User data not found", ResponseStatus.NotFound);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }


    private async Task<(IEnumerable<FleetResponseDto>, IEnumerable<Guid>)> GetFleetsDtos(IEnumerable<Fleet>? fleets, IEnumerable<Guid>? fleetsRequest)
    {
        List<FleetResponseDto> fleetsDto = [];
        List<Guid> unknownFleets = fleetsRequest?.Where(fr => fleets is null || !fleets.Any(f => f.Id == fr)).ToList() ?? [];

        if (fleets is not null && fleets.Any())
        {    
            foreach (var fleet in fleets)
            {
                if (fleetsRequest is not null && fleetsRequest.Any() && !fleetsRequest.Contains(fleet.Id))
                { 
                    continue;
                }

                var staffFleetRegistrationNumber = await DecryptAsync(fleet.EncryptedRegistrationNumber);
                var staffFleetChassisNumber = await DecryptAsync(fleet.EncryptedChassisNumber);
                var staffFleetEngineNumber = await DecryptAsync(fleet.EncryptedEngineNumber);
                var staffFleetInsuranceNumber = await DecryptAsync(fleet.EncryptedInsuranceNumber);
                var staffFleetOwnerName = await DecryptAsync(fleet.Owner.EncryptedOwnerName);
                var staffFleetOwnerAddress = await DecryptAsync(fleet.Owner.EncryptedOwnerAddress);

                fleetsDto.Add(new FleetResponseDto(
                    fleet.Id,
                    staffFleetRegistrationNumber,
                    fleet.VehicleType,
                    fleet.Brand,
                    fleet.Model,
                    fleet.YearOfManufacture,
                    staffFleetChassisNumber,
                    staffFleetEngineNumber,
                    staffFleetInsuranceNumber,
                    fleet.IsInsuranceValid,
                    fleet.LastInspectionDateUtc,
                    fleet.OdometerReading,
                    fleet.FuelType,
                    staffFleetOwnerName,
                    staffFleetOwnerAddress,
                    fleet.UsageStatus,
                    fleet.OwnershipStatus,
                    fleet.TransmissionType,
                    fleet.ImageUrl));
            }
        }

        return (fleetsDto, unknownFleets);
    }


    private async Task<string> DecryptAsync(string cipherText)
    {
        return await _kmsClient.DecryptAsync(cipherText);
    }

    private async Task<string?> DecryptNullableAsync(string? cipherText)
    {
        if (cipherText == null) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}

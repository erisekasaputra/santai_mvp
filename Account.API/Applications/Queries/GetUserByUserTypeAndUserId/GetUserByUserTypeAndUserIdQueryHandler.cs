using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using Core.Configurations;
using Core.Enumerations;
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
            dynamic? user = request.UserType switch
            {
                UserType.RegularUser => await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId),
                UserType.MechanicUser => await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId), 
                UserType.BusinessUser => await _unitOfWork.BaseUsers.GetBusinessUserByIdAsync(request.UserId),
                UserType.StaffUser => await _unitOfWork.Staffs.GetByIdAsync(request.UserId),
                _ => throw new NotImplementedException()
            }; 
            
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }
             
            if (user is RegularUser regularUser)
            {
                var decryptedEmail = await DecryptNullableAsync(regularUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(regularUser.EncryptedPhoneNumber);
                (var fleets, var fleetsUnknown) = await GetFleetsDtos(regularUser.Fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    regularUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    regularUser.TimeZoneId,
                    regularUser.PersonalInfo.ToString() ?? string.Empty,
                    fleets,
                    fleetsUnknown);

                return Result.Success(userResponseDto, ResponseStatus.Ok); ;
            }
            else if (user is BusinessUser businessUser)
            {
                var decryptedEmail = await DecryptNullableAsync(businessUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(businessUser.EncryptedPhoneNumber);
                (var fleets, var fleetsUnknown) = await GetFleetsDtos(businessUser.Fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    businessUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    businessUser.TimeZoneId,
                    businessUser.BusinessName,
                    fleets,
                    fleetsUnknown);

                return Result.Success(userResponseDto, ResponseStatus.Ok);
            }
            else if (user is Staff staffUser)
            {
                var decryptedEmail = await DecryptNullableAsync(staffUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(staffUser.EncryptedPhoneNumber);
                (var fleets, var fleetsUnknown) = await GetFleetsDtos(staffUser.Fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    staffUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    staffUser.TimeZoneId,
                    staffUser.Name,
                    fleets,
                    fleetsUnknown);

                return Result.Success(userResponseDto, ResponseStatus.Ok);
            }
            else if (user is MechanicUser mechanicUser)
            {
                var decryptedEmail = await DecryptNullableAsync(mechanicUser.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(mechanicUser.EncryptedPhoneNumber);  
                (var fleets, var fleetsUnknown) = await GetFleetsDtos(mechanicUser.Fleets, request.Fleets);

                var userResponseDto = new GlobalUserResponseDto(
                    mechanicUser.Id,
                    decryptedEmail,
                    decryptedPhoneNumber,
                    mechanicUser.TimeZoneId,
                    mechanicUser.PersonalInfo.ToString() ?? string.Empty,
                    fleets,
                    fleetsUnknown);

                return Result.Success(userResponseDto, ResponseStatus.Ok);
            }
            else
            {
                return Result.Failure($"User {request.UserType} is not supported", ResponseStatus.BadRequest);
            } 
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

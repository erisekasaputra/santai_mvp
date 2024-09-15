using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services; 
using Account.API.Extensions;
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR; 
using Core.Services.Interfaces;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public class GetRegularUserByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient) : IRequestHandler<GetRegularUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;  

    public async Task<Result> Handle(GetRegularUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        { 
            var user = await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            var userDto = await ToRegularUserResponseDto(user, request.FleetsRequest?.FleetIds);
             
            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<RegularUserResponseDto> ToRegularUserResponseDto(RegularUser user, IEnumerable<Guid>? fleetIds = null)
    {
        var decryptedEmail = await DecryptNullableAsync(user.EncryptedEmail);
        var decryptedPhoneNumber = await DecryptNullableAsync(user.EncryptedPhoneNumber);
        var decryptedAddressLine1 = await DecryptAsync(user.Address.EncryptedAddressLine1);
        var decryptedAddressLine2 = await DecryptNullableAsync(user.Address.EncryptedAddressLine2);
        var decryptedAddressLine3 = await DecryptNullableAsync(user.Address.EncryptedAddressLine3);

        List<FleetResponseDto> fleets = [];

        if (user.Fleets is not null && user.Fleets.Count > 0) 
        { 
            foreach(var fleet in user.Fleets)
            {
                if (fleetIds is not null && fleetIds.Any() && !fleetIds.Contains(fleet.Id))
                {
                    continue;
                } 

                var registrationNumber = await DecryptAsync(fleet.EncryptedRegistrationNumber);
                var chassisNumber = await DecryptAsync(fleet.EncryptedChassisNumber);
                var engineNumber = await DecryptAsync(fleet.EncryptedEngineNumber);
                var insuranceNumber = await DecryptAsync(fleet.EncryptedInsuranceNumber);
                var ownerName = await DecryptAsync(fleet.Owner.EncryptedOwnerName);
                var ownerAddress = await DecryptAsync(fleet.Owner.EncryptedOwnerAddress);

                fleets.Add(new FleetResponseDto(
                    fleet.Id,
                    registrationNumber,
                    fleet.VehicleType,
                    fleet.Brand,
                    fleet.Model,
                    fleet.YearOfManufacture,
                    chassisNumber,
                    engineNumber,
                    insuranceNumber,
                    fleet.IsInsuranceValid,
                    fleet.LastInspectionDateUtc,
                    fleet.OdometerReading,
                    fleet.FuelType,
                    ownerName,
                    ownerAddress,
                    fleet.UsageStatus,
                    fleet.OwnershipStatus,
                    fleet.TransmissionType,
                    fleet.ImageUrl));
            }
        } 

        var address = new AddressResponseDto(
            decryptedAddressLine1,
            decryptedAddressLine2,
            decryptedAddressLine3,
            user.Address.City,
            user.Address.State,
            user.Address.PostalCode,
            user.Address.Country);
         
        return new RegularUserResponseDto(
                user.Id,  
                decryptedEmail,
                decryptedPhoneNumber,
                user.TimeZoneId,
                address,
                user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
                user.ReferralProgram.ToReferralProgramResponseDto(),
                user.PersonalInfo.ToPersonalInfoResponseDto(user.TimeZoneId),
                fleets);
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

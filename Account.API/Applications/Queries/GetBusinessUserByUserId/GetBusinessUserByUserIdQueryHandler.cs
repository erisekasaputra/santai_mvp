using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public class GetBusinessUserByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,  
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetBusinessUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IEncryptionService _kmsClient = kmsClient;   
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetBusinessUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var fleetIds = request.FleetsRequest?.Fleets;

            var user = await _unitOfWork.BaseUsers.GetBusinessUserByIdAsync(request.Id); 
            if (user is null)
            {
                return Result.Failure($"Business user '{request.Id}' is not found", ResponseStatus.NotFound);
            }

            var decryptedEmail = await DecryptNullableAsync(user.EncryptedEmail);
            var decryptedPhoneNumber = await DecryptNullableAsync(user.EncryptedPhoneNumber);
            var decryptedContactPerson = await DecryptAsync(user.EncryptedContactPerson);
            var decryptedTaxId = await DecryptNullableAsync(user.EncryptedTaxId);
            var decryptedAddressLine1 = await DecryptAsync(user.Address.EncryptedAddressLine1);
            var decryptedAddressLine2 = await DecryptNullableAsync(user.Address.EncryptedAddressLine2);
            var decryptedAddressLine3 = await DecryptNullableAsync(user.Address.EncryptedAddressLine3);

            var address = new AddressResponseDto( 
                    decryptedAddressLine1, 
                    decryptedAddressLine2, 
                    decryptedAddressLine3,
                    user.Address.City,
                    user.Address.State,
                    user.Address.PostalCode,
                    user.Address.Country
                );

            var businessLicenseResponses = new List<BusinessLicenseResponseDto>();

            if (user.BusinessLicenses is not null && user.BusinessLicenses.Count > 0)
            {
                foreach (var businessLicense in user.BusinessLicenses)
                {
                    var decryptedLicenseNumber = await DecryptAsync(businessLicense.EncryptedLicenseNumber);

                    businessLicenseResponses.Add(new BusinessLicenseResponseDto(
                            // from existing entity
                            businessLicense.Id,
                            // decrypted from existing entity
                            decryptedLicenseNumber,
                            // from existing entity
                            businessLicense.Name,
                            // from existing entity
                            businessLicense.Description
                        ));
                }
            }


            var staffResponses = new List<StaffResponseDto>();

            if (user.Staffs is not null && user.Staffs.Count > 0)
            {
                foreach (var staff in user.Staffs)
                {
                    var decryptedStaffPhoneNumber = await DecryptNullableAsync(staff.EncryptedPhoneNumber); 
                    var decryptedStaffEmail = await DecryptNullableAsync(staff.EncryptedEmail);

                    var decryptedStaffAddressLine1 = await DecryptAsync(staff.Address.EncryptedAddressLine1); 
                    var decryptedStaffAddressLine2 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine2); 
                    var decryptedStaffAddressLine3 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine3); 

                    var staffAddress = new AddressResponseDto(
                         decryptedStaffAddressLine1,
                         decryptedStaffAddressLine2,
                         decryptedStaffAddressLine3,
                         staff.Address.City,
                         staff.Address.State,
                         staff.Address.PostalCode,
                         staff.Address.Country
                    );


                    List<FleetResponseDto> staffFleets = [];

                    if (staff.Fleets is not null && staff.Fleets.Count > 0)
                    {
                        foreach (var staffFleet in staff.Fleets)
                        {
                            if (fleetIds is not null && fleetIds.Any() && !fleetIds.Contains(staffFleet.Id))
                            {
                                continue;
                            }

                            var staffFleetRegistrationNumber = await DecryptNullableAsync(staffFleet.EncryptedRegistrationNumber);
                            var staffFleetChassisNumber = await DecryptNullableAsync(staffFleet.EncryptedChassisNumber);
                            var staffFleetEngineNumber = await DecryptNullableAsync(staffFleet.EncryptedEngineNumber);
                            var staffFleetInsuranceNumber = await DecryptNullableAsync(staffFleet.EncryptedInsuranceNumber);
                            var staffFleetOwnerName = await DecryptNullableAsync(staffFleet.Owner?.EncryptedOwnerName);
                            var staffFleetOwnerAddress = await DecryptNullableAsync(staffFleet.Owner?.EncryptedOwnerAddress);

                            staffFleets.Add(new FleetResponseDto(
                                staffFleet.Id,
                                staffFleetRegistrationNumber,
                                staffFleet.VehicleType,
                                staffFleet.Brand,
                                staffFleet.Model,
                                staffFleet.YearOfManufacture,
                                staffFleetChassisNumber,
                                staffFleetEngineNumber,
                                staffFleetInsuranceNumber,
                                staffFleet.IsInsuranceValid,
                                staffFleet.LastInspectionDateUtc,
                                staffFleet.OdometerReading,
                                staffFleet.FuelType,
                                staffFleetOwnerName,
                                staffFleetOwnerAddress,
                                staffFleet.UsageStatus,
                                staffFleet.OwnershipStatus,
                                staffFleet.TransmissionType,
                                staffFleet.ImageUrl));
                        }
                    }

                    staffResponses.Add(new StaffResponseDto( 
                            staff.Id, 
                            decryptedStaffEmail,
                            decryptedStaffPhoneNumber,
                            staff.Name,
                            staff.ImageUrl,
                            staffAddress,
                            staff.TimeZoneId,
                            staffFleets));
                }
            }


            List<FleetResponseDto> fleets = [];

            if (user.Fleets is not null && user.Fleets.Count > 0)
            {
                foreach (var fleet in user.Fleets)
                {
                    if (fleetIds is not null && fleetIds.Any() && !fleetIds.Contains(fleet.Id))
                    {
                        continue;
                    }

                    var registrationNumber = await DecryptNullableAsync(fleet.EncryptedRegistrationNumber);
                    var chassisNumber = await DecryptNullableAsync(fleet.EncryptedChassisNumber);
                    var engineNumber = await DecryptNullableAsync(fleet.EncryptedEngineNumber);
                    var insuranceNumber = await DecryptNullableAsync(fleet.EncryptedInsuranceNumber);
                    var ownerName = await DecryptNullableAsync(fleet.Owner?.EncryptedOwnerName);
                    var ownerAddress = await DecryptNullableAsync(fleet.Owner?.EncryptedOwnerAddress);

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


            var userDto = new BusinessUserResponseDto(
                user.Id, 
                decryptedEmail,
                decryptedPhoneNumber,
                user.TimeZoneId,
                address,
                user.BusinessName,
                user.BusinessImageUrl,
                decryptedContactPerson,
                decryptedTaxId,
                user.WebsiteUrl,
                user.Description,
                user.LoyaltyProgram.ToLoyaltyProgramResponseDto(), 
                user.ReferralProgram.ToReferralProgramResponseDto(),
                fleets,
                businessLicenseResponses,
                staffResponses); 

            return Result.Success(userDto); 
        }
        catch (Exception ex) 
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
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

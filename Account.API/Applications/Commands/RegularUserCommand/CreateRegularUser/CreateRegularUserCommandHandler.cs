using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Core.Results;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;
using Core.Configurations;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;

public class CreateRegularUserCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IOptionsMonitor<ReferralProgramConfiguration> referralOptions,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<CreateRegularUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service; 
    private readonly IOptionsMonitor<ReferralProgramConfiguration> _referralOptions = referralOptions;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(CreateRegularUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var addressRequest = request.Address;

            var hashedEmail = await HashNullableAsync(request.Email);
            var hashedPhoneNumber = await HashAsync(request.PhoneNumber);

            var encryptedEmail = await EncryptNullableAsync(request.Email);
            var encryptedPhoneNumber = await EncryptAsync(request.PhoneNumber);

            var encryptedAddressLine1 = await EncryptAsync(addressRequest.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(addressRequest.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(addressRequest.AddressLine3);

            var address = new Address(
                  encryptedAddressLine1,
                  encryptedAddressLine2,
                  encryptedAddressLine3,
                  addressRequest.City,
                  addressRequest.State,
                  addressRequest.PostalCode,
                  addressRequest.Country);

            if (await _unitOfWork.BaseUsers.GetAnyByIdAsync(request.IdentityId))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("User already registered", ResponseStatus.Conflict);
            }

            var conflicts = await _unitOfWork.BaseUsers.GetByIdentitiesAsNoTrackingAsync(
                    (IdentityParameter.Email, hashedEmail),
                    (IdentityParameter.PhoneNumber, hashedPhoneNumber));
             
            if (conflicts is not null)
            {
                return await RollbackAndReturnFailureAsync(UserIdentityConflict(
                    conflicts, 
                    hashedEmail,
                    hashedPhoneNumber,
                    request.IdentityId), cancellationToken);
            } 
             

            var user = new RegularUser(
                request.IdentityId, 
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
                address,
                request.PersonalInfo.ToPersonalInfo(request.TimeZoneId),
                request.TimeZoneId );

            // creating referral program if exists
            int? referralRewardPoint = _referralOptions.CurrentValue.Point;
            int? referralValidMonth = _referralOptions.CurrentValue.ValidMonth;
            if (referralRewardPoint.HasValue && referralValidMonth.HasValue)
            {
                user.AddReferralProgram(referralRewardPoint.Value, referralRewardPoint.Value);
            }

            // create referred programs when user input the referral code and referral code is valid
            if (!string.IsNullOrEmpty(request.ReferralCode))
            {
                // check is referral code is valid
                var referralProgram = await _unitOfWork.ReferralPrograms.GetByCodeAsync(request.ReferralCode);
                if (referralProgram is null)
                {
                    return await RollbackAndReturnFailureAsync(
                        Result.Failure("Referral code is invalid", ResponseStatus.BadRequest)
                            .WithError(new("RegularUser.ReferralCode", "Referral code is invalid")), cancellationToken);
                }

                // check is referral program is still valid 
                if (referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    return await RollbackAndReturnFailureAsync(
                        Result.Failure("Referral code is expired", ResponseStatus.BadRequest)
                            .WithError(new("RegularUser.ReferralCode", "Referral code is expired")), cancellationToken);
                }

                // creating the referred programs
                await _unitOfWork.ReferredPrograms.CreateReferredProgramAsync(
                    new ReferredProgram(
                        referralProgram.UserId,
                        user.Id,
                        request.ReferralCode,
                        DateTime.UtcNow));
            }

            await _unitOfWork.BaseUsers.CreateAsync(user);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return Result.Success(
                await ToRegularUserResponseDto(user, request), ResponseStatus.Created);
        }

        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(Result.Failure(ex.Message, ResponseStatus.BadRequest), cancellationToken);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError), cancellationToken);
        }
    }

    private async Task<RegularUserResponseDto> ToRegularUserResponseDto(
        RegularUser user, 
        CreateRegularUserCommand request)
    {
        var addressResponseDto = new AddressResponseDto(
                request.Address.AddressLine1,
                request.Address.AddressLine2,
                request.Address.AddressLine3,
                request.Address.City,
                request.Address.State,
                request.Address.PostalCode,
                request.Address.Country);

        var personalInfoResponseDto = new PersonalInfoResponseDto(
            request.PersonalInfo.FirstName,
            request.PersonalInfo.MiddleName,
            request.PersonalInfo.LastName,
            request.PersonalInfo.DateOfBirth,
            request.PersonalInfo.Gender,
            request.PersonalInfo.ProfilePictureUrl);

        List<FleetResponseDto> fleets = [];

        if (user.Fleets is not null && user.Fleets.Count > 0)
        {
            foreach (var fleet in user.Fleets)
            { 
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

        var regularUserResponseDto = new RegularUserResponseDto(
            user.Id, 
            request.Email,
            request.PhoneNumber,
            user.TimeZoneId,
            addressResponseDto,
            user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
            user.ReferralProgram.ToReferralProgramResponseDto(),
            personalInfoResponseDto,
            fleets);

        return regularUserResponseDto;
    }

    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return result;
    }

    private static Result UserIdentityConflict(BaseUser user, string? hashedEmail, string hashedPhoneNumber, Guid identityId)
    {
        var conflictIdentities = new List<ErrorDetail>(); 

        if (!string.IsNullOrWhiteSpace(hashedEmail))
        {
            if (user.HashedEmail == hashedEmail || user.NewHashedEmail == hashedEmail)
            {
                conflictIdentities.Add(new ($"RegularUser.{nameof(user.HashedEmail)}", 
                    "Email already registered"));
            } 
        }

        if (user.HashedPhoneNumber == hashedPhoneNumber || user.NewHashedPhoneNumber == hashedPhoneNumber)
        {
            conflictIdentities.Add(new ($"RegularUser.{nameof(user.HashedPhoneNumber)}", 
                "Phone number already registered"));
        } 

        var message = conflictIdentities.Count == 1
            ? "There is a conflict"
            : $"There are {conflictIdentities.Count} conflicts";

        return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(conflictIdentities);
    }

    private async Task<string?> EncryptNullableAsync(string? value)
    {
        if (value == null) return null;

        return await _kmsClient.EncryptAsync(value);
    }

    private async Task<string> EncryptAsync(string value)
    {
        return await _kmsClient.EncryptAsync(value);
    }

    private async Task<string?> DecryptNullableAsync(string? value)
    {
        if (value == null) return null;

        return await _kmsClient.DecryptAsync(value);
    }

    private async Task<string> DecryptAsync(string value)
    {
        return await _kmsClient.DecryptAsync(value);
    }


    private async Task<string> HashAsync(string value)
    {
        return await _hashClient.Hash(value);
    }
    private async Task<string?> HashNullableAsync(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return await _hashClient.Hash(value);
    }  
}

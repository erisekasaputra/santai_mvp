using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services; 
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data; 

namespace Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;

public class CreateRegularUserCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IOptionsMonitor<ReferralProgramOption> referralOptions,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<CreateRegularUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service; 
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions = referralOptions;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(CreateRegularUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var addressRequest = request.Address;

            var hashedEmail = await HashAsync(request.Email);
            var hashedPhoneNumber = await HashAsync(request.PhoneNumber);

            var encryptedEmail = await EncryptAsync(request.Email);
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

            var conflicts = await _unitOfWork.Users.GetByIdentitiesAsNoTrackingAsync(
                    (IdentityParameter.Email, hashedEmail),
                    (IdentityParameter.PhoneNumber, hashedPhoneNumber),
                    (IdentityParameter.Username, request.Username),
                    (IdentityParameter.IdentityId, request.IdentityId.ToString())
                );

            if (conflicts is not null)
            {
                return await RollbackAndReturnFailureAsync(UserIdentityConflict(
                    conflicts,
                    request.Username,
                    hashedEmail,
                    hashedPhoneNumber,
                    request.IdentityId), cancellationToken);
            } 
             

            var user = new RegularUser(
                request.IdentityId,
                request.Username,
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
                address,
                request.PersonalInfo.ToPersonalInfo(request.TimeZoneId),
                request.TimeZoneId,
                request.DeviceId);

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
                        Result.Failure("Referral code is invalid", ResponseStatus.BadRequest), cancellationToken);
                }

                // check is referral program is still valid 
                if (referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    return await RollbackAndReturnFailureAsync(
                        Result.Failure("Referral code is expired", ResponseStatus.BadRequest), cancellationToken);
                }

                // creating the referred programs
                await _unitOfWork.ReferredPrograms.CreateReferredProgramAsync(
                    new ReferredProgram(
                        referralProgram.UserId,
                        user.Id,
                        request.ReferralCode,
                        DateTime.UtcNow));
            }

            await _unitOfWork.Users.CreateAsync(user);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return Result.Success(ToRegularUserResponseDto(user, request), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(Result.Failure(ex.Message, ResponseStatus.BadRequest), cancellationToken);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError), cancellationToken);
        }
    }

    private static RegularUserResponseDto ToRegularUserResponseDto(RegularUser user, CreateRegularUserCommand request)
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

        var regularUserResponseDto = new RegularUserResponseDto(
            user.Id,
            user.Username,
            request.Email,
            request.PhoneNumber,
            user.TimeZoneId,
            addressResponseDto,
            personalInfoResponseDto);

        return regularUserResponseDto;
    }

    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return result;
    }

    private static Result UserIdentityConflict(User user, string username, string hashedEmail, string hashedPhoneNumber, Guid identityId)
    {
        var conflictIdentities = new List<ErrorDetail>();

        if (user.Username == username)
        {
            conflictIdentities.Add(new ($"User.{nameof(user.Username)}", 
                "Username already registered"));
        }

        if (user.HashedEmail == hashedEmail || user.NewHashedEmail == hashedEmail)
        {
            conflictIdentities.Add(new ($"User.{nameof(user.HashedEmail)}", 
                "Email already registered"));
        }

        if (user.HashedPhoneNumber == hashedPhoneNumber || user.NewHashedPhoneNumber == hashedPhoneNumber)
        {
            conflictIdentities.Add(new ($"User.{nameof(user.HashedPhoneNumber)}", 
                "Phone number already registered"));
        }

        if (user.IdentityId == identityId)
        {
            conflictIdentities.Add(new ($"User.{nameof(user.IdentityId)}", 
                "Identity id already registered"));
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

    private async Task<string> HashAsync(string value)
    {
        return await _hashClient.Hash(value);
    }
}

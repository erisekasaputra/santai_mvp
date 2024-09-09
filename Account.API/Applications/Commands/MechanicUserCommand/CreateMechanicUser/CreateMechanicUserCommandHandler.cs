using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services; 
using Account.API.Extensions;
using Core.Results;
using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;
using Core.Configurations;
using Core.Extensions;
using Core.Messages;
using Core.Services.Interfaces;

namespace Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;

public class CreateMechanicUserCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IOptionsMonitor<ReferralProgramConfiguration> referralOption,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<CreateMechanicUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IOptionsMonitor<ReferralProgramConfiguration> _referralOptions = referralOption;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;


    public async Task<Result> Handle(CreateMechanicUserCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var errors = new List<ErrorDetail>();

            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
             
            var hashedEmail = await HashNullableAsync(request.Email);
            var hashedPhoneNumber = await HashAsync(request.PhoneNumber);

            var encryptedEmail = await EncryptNullableAsync(request.Email);
            var encryptedPhoneNumber = await EncryptAsync(request.PhoneNumber);

            var encryptedAddressLine1 = await EncryptAsync(request.Address.AddressLine1);
            var encryptedAddressLine2 = await EncryptNullableAsync(request.Address.AddressLine2);
            var encryptedAddressLine3 = await EncryptNullableAsync(request.Address.AddressLine3);

            var hashedLicenseNumber = await HashAsync(request.DrivingLicense.LicenseNumber);
            var encryptedLicenseNumber = await EncryptAsync(request.DrivingLicense.LicenseNumber);
            var hashedIdentityNumber = await HashAsync(request.NationalIdentity.IdentityNumber);
            var encryptedIdentityNumber = await EncryptAsync(request.NationalIdentity.IdentityNumber);

            if (await _unitOfWork.BaseUsers.GetAnyByIdAsync(request.IdentityId))
            {
                return Result.Failure("User already registered", ResponseStatus.Conflict);
            }

            // get all users that already registered with related request identities such as email, username, phonenumber, and identity id(from identity database)
            var conflictUsers = await _unitOfWork.BaseUsers.GetByIdentitiesAsNoTrackingAsync( 
                (IdentityParameter.Email, hashedEmail),
                (IdentityParameter.PhoneNumber, hashedPhoneNumber));


            // check if user with conlict identities is not null
            if (conflictUsers is not null)
            {
                // if it is not null, rollback the trasaction and get the conflict items 
                errors.AddRange(UserIdentityConflict(
                    conflictUsers,
                    request.IdentityId, 
                    hashedEmail,
                    hashedPhoneNumber));
            }


            var isNationalIdentityRegistered = await _unitOfWork.NationalIdentities.GetAnyByIdentityNumberAsync(hashedIdentityNumber);  
            var isDrivingLicenseRegistered = await _unitOfWork.DrivingLicenses.GetAnyByLicenseNumberAsync(hashedLicenseNumber);

            if (isNationalIdentityRegistered || isDrivingLicenseRegistered)
            {  
                if (isNationalIdentityRegistered)
                {
                    errors.Add(new ("NationalIdentity.IdentityNumber", "National identity already registered"));
                }

                if (isDrivingLicenseRegistered)
                {
                    errors.Add(new("DrivingLicense.LicenseNumber", "License number already registered"));
                } 
            }  

            var certificationIds = request.Certifications.Select(x => x.CertificationId);
            var conflictCertifications = await _unitOfWork.Certifications
                .GetByCertIdsAsNoTrackingAsync(certificationIds.ToArray());


            if (conflictCertifications is not null && conflictCertifications.Any())
            {
                errors.AddRange(CertificationConflicts(conflictCertifications, request.Certifications));
            }

            ReferralProgram? referralProgram = null;

            // create referred programs when user input the referral code and referral code is valid
            if (!string.IsNullOrEmpty(request.ReferralCode))
            {
                // check is referral code is valid
                referralProgram = await _unitOfWork.ReferralPrograms.GetByCodeAsync(request.ReferralCode);
                if (referralProgram is null)
                {
                    errors.Add(new ErrorDetail("MechanicUser.ReferralCode", "Referral code is invalid")); 
                }

                // check is referral program is still valid 
                if (referralProgram is not null && referralProgram.ValidDateUtc < DateTime.UtcNow)
                {
                    errors.Add(new ErrorDetail("MechanicUser.ReferralCode", "Referral code is expired")); 
                }  
            }



            // middleware for returning error if error occured
            if (errors.Count > 0)
            {
                return await RollbackAndReturnFailureAsync(
                    Result.Failure($"There {(errors.Count <= 1 ? "is" : "are" )} few error(s) that you have to fixed", 
                    ResponseStatus.BadRequest).WithErrors(errors), cancellationToken);
            } 


            var address = new Address(
                    encryptedAddressLine1,
                    encryptedAddressLine2,
                    encryptedAddressLine3,
                    request.Address.City,
                    request.Address.State,
                    request.Address.PostalCode,
                    request.Address.Country);


            var user = new MechanicUser(
                request.IdentityId, 
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
                request.PersonalInfo.ToPersonalInfo(request.TimeZoneId),
                address,
                request.TimeZoneId,
                request.DeviceId);
            
            
            int? referralRewardPoint = _referralOptions.CurrentValue.Point;
            int? referralValidMonth = _referralOptions.CurrentValue.ValidMonth;


            // Registering referral program
            if (referralRewardPoint.HasValue && referralValidMonth.HasValue)
            {
                user.AddReferralProgram(referralRewardPoint.Value, referralRewardPoint.Value);
            } 

            if (request.Certifications.Any())
            {
                foreach (var certification in request.Certifications)
                {  
                    user.AddCertification(
                        certification.CertificationId,
                        certification.CertificationName,
                        certification.ValidDate.FromLocalToUtc(request.TimeZoneId),
                        certification.Specialization?.ToList());
                }
            }   
          
            user.SetDrivingLicense(
                hashedLicenseNumber,
                encryptedLicenseNumber,
                request.DrivingLicense.FrontSideImageUrl,
                request.DrivingLicense.BackSideImageUrl); 

            user.SetNationalID(
                hashedIdentityNumber,
                encryptedIdentityNumber,
                request.NationalIdentity.FrontSideImageUrl,
                request.NationalIdentity.BackSideImageUrl);

            if (referralProgram is not null && request.ReferralCode is not null)
            {
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

            return Result.Success(ToMechanicResponseDto(user, request), 
                ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return await RollbackAndReturnFailureAsync(
                Result.Failure(ex.Message, ResponseStatus.BadRequest), cancellationToken); 
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex, ex.InnerException?.Message);
            return await RollbackAndReturnFailureAsync(
                Result.Failure(Messages.InternalServerError, ResponseStatus.BadRequest), cancellationToken); 
        }
    }

    private static List<ErrorDetail> CertificationConflicts(
      IEnumerable<Certification> conflicts,
      IEnumerable<CertificationRequestDto> certifications)
    {
        var errors = certifications
        .SelectMany((certification, index) =>
            conflicts.Any(x => x.CertificationId == certification.CertificationId)
                ? [ new ErrorDetail($"Certification[{index}].{nameof(certification.CertificationId)}",
                    "Certificate id already registered") ]
                : Array.Empty<ErrorDetail>()
        ).ToList();

        return errors; 
    }


    private static List<ErrorDetail> UserIdentityConflict(
        BaseUser user,
        Guid identityId, 
        string? email,
        string phoneNumber)
    {
        var conflicts = new List<ErrorDetail>(); 
        
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (user.HashedEmail == email || user.NewHashedEmail == email)
            {
                conflicts.Add(new($"MechanicUser.{nameof(user.HashedEmail)}", 
                    "User email already registered"));
            } 
        }

        if (user.HashedPhoneNumber == phoneNumber || user.NewHashedPhoneNumber == phoneNumber)
        {
            conflicts.Add(new($"MechanicUser.{nameof(user.HashedPhoneNumber)}",
                "User phone number already registered"));
        }

        return conflicts;
    }


    private async Task<Result> RollbackAndReturnFailureAsync(Result result, CancellationToken cancellationToken)
    {
        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
        return result;
    }

    private static MechanicUserResponseDto ToMechanicResponseDto(MechanicUser user, CreateMechanicUserCommand request)
    {
        var addressResponseDto = new AddressResponseDto(
                request.Address.AddressLine1,
                request.Address.AddressLine2,
                request.Address.AddressLine3,
                request.Address.City,
                request.Address.State,
                request.Address.PostalCode,
                request.Address.Country);

        var certificatonResponseDto = new List<CertificationResponseDto>();
        if (request.Certifications is not null && request.Certifications.Any())
        {
            foreach (var certification in request.Certifications)
            {
                certificatonResponseDto.Add(new CertificationResponseDto(
                    certification.CertificationId,
                    certification.CertificationName,
                    certification.ValidDate,
                    certification.Specialization));
            }
        }

        var drivingLicenseId = user.DrivingLicenses?.FirstOrDefault();
        var nationalIdentityId = user.NationalIdentities?.FirstOrDefault();


        DrivingLicenseResponseDto? drivingLicenseResponseDto = null;
        
        if (drivingLicenseId is not null)
        { 
            drivingLicenseResponseDto = new DrivingLicenseResponseDto(
                drivingLicenseId.Id,
                request.DrivingLicense.LicenseNumber,
                request.DrivingLicense.FrontSideImageUrl,
                request.DrivingLicense.BackSideImageUrl);
        }


        NationalIdentityResponseDto? nationalIdResponseDto = null; 
        
        if (nationalIdentityId is not null)
        {
            nationalIdResponseDto = new NationalIdentityResponseDto(
                nationalIdentityId.Id,
                request.NationalIdentity.IdentityNumber,
                request.NationalIdentity.FrontSideImageUrl,
                request.NationalIdentity.BackSideImageUrl);
        } 

        var mechanicResponse = new MechanicUserResponseDto(
            user.Id, 
            request.Email,
            request.PhoneNumber,
            request.TimeZoneId,
            user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
            addressResponseDto,
            certificatonResponseDto,
            drivingLicenseResponseDto,
            nationalIdResponseDto);

        return mechanicResponse;
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

    private async Task<string?> HashNullableAsync(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return await _hashClient.Hash(value);
    }
}

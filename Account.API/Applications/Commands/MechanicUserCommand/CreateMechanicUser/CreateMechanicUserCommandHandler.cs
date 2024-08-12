using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;

public class CreateMechanicUserCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IOptionsMonitor<ReferralProgramOption> referralOption,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<CreateMechanicUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions = referralOption;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(CreateMechanicUserCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);

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

            var user = new MechanicUser(
                request.IdentityId,
                request.Username,
                hashedEmail,
                encryptedEmail,
                hashedPhoneNumber,
                encryptedPhoneNumber,
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
            
            var drivingLicense = request.DrivingLicenseRequestDto;
            var hashedLicenseNumber = await HashAsync(drivingLicense.LicenseNumber);
            var encryptedLicenseNumber = await EncryptAsync(drivingLicense.LicenseNumber);
            user.SetDrivingLicense(
                hashedLicenseNumber,
                encryptedLicenseNumber,
                drivingLicense.FrontSideImageUrl,
                drivingLicense.BackSideImageUrl);

            var nationalId = request.NationalIdentityRequestDto;
            var hashedIdentityNumber = await HashAsync(nationalId.IdentityNumber);
            var encryptedIdentityNumber = await EncryptAsync(nationalId.IdentityNumber);
            user.SetNationalID(
                hashedIdentityNumber,
                encryptedIdentityNumber,
                nationalId.FrontSideImageUrl,
                nationalId.BackSideImageUrl);

            await _unitOfWork.Users.CreateAsync(user);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);  

            return Result.Success(ToMechanicResponseDto(user, request), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
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
                request.DrivingLicenseRequestDto.LicenseNumber,
                request.DrivingLicenseRequestDto.FrontSideImageUrl,
                request.DrivingLicenseRequestDto.BackSideImageUrl);
        }


        NationalIdentityResponseDto? nationalIdResponseDto = null; 
        
        if (nationalIdentityId is not null)
        {
            nationalIdResponseDto = new NationalIdentityResponseDto(
                nationalIdentityId.Id,
                request.NationalIdentityRequestDto.IdentityNumber,
                request.NationalIdentityRequestDto.FrontSideImageUrl,
                request.NationalIdentityRequestDto.BackSideImageUrl);
        } 

        var mechanicResponse = new MechanicUserResponseDto(
            user.Username,
            request.Email,
            request.PhoneNumber,
            request.TimeZoneId,
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
}

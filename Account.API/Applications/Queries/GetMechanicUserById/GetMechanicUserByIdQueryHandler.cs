using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Core.Results;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetMechanicUserById;

public class GetMechanicUserByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetMechanicUserByIdQuery, Result>
{ 
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;

    public async Task<Result> Handle(GetMechanicUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {  
            var user = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"Mechanic user '{request.Id}' not found", ResponseStatus.NotFound);
            }

            var address = await DecryptAddressAsync(user.Address, cancellationToken);
            
            var certifications = user.Certifications?.Select(cert => cert.ToCertificationResponseDto(user.TimeZoneId)).ToList() ?? [];

            var nationalIdentity = await DecryptNationalIdentityAsync(
                user.NationalIdentities?
                    .OrderByDescending(x => x.VerificationStatus == VerificationState.Accepted)
                    .FirstOrDefault()
                , cancellationToken);
            
            var drivingLicense = await DecryptDrivingLicenseAsync(
                user.DrivingLicenses?
                    .OrderByDescending(x => x.VerificationStatus == VerificationState.Accepted)
                    .FirstOrDefault()
                , cancellationToken);

            var userDto = new MechanicUserResponseDto(
                user.Id, 
                await DecryptNullableAsync(user.EncryptedEmail),
                await DecryptNullableAsync(user.EncryptedPhoneNumber),
                user.TimeZoneId,
                user.PersonalInfo.ToPersonalInfoResponseDto(user.TimeZoneId),
                user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
                user.ReferralProgram.ToReferralProgramResponseDto(),
                address, 
                certifications,
                drivingLicense,
                nationalIdentity
            );
             
            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<AddressResponseDto> DecryptAddressAsync(Address address, CancellationToken cancellationToken)
    {
        return new AddressResponseDto(
            await DecryptAsync(address.EncryptedAddressLine1),
            await DecryptNullableAsync(address.EncryptedAddressLine2),
            await DecryptNullableAsync(address.EncryptedAddressLine3),
            address.City,
            address.State,
            address.PostalCode,
            address.Country
        );
    }

    private async Task<NationalIdentityResponseDto?> DecryptNationalIdentityAsync(NationalIdentity? nationalIdentity, CancellationToken cancellationToken)
    {
        if (nationalIdentity is null) return null;

        return new NationalIdentityResponseDto(
            nationalIdentity.Id,
            await DecryptAsync(nationalIdentity.EncryptedIdentityNumber),
            nationalIdentity.FrontSideImageUrl,
            nationalIdentity.BackSideImageUrl
        );
    }

    private async Task<DrivingLicenseResponseDto?> DecryptDrivingLicenseAsync(DrivingLicense? drivingLicense, CancellationToken cancellationToken)
    {
        if (drivingLicense is null) return null;

        return new DrivingLicenseResponseDto(
            drivingLicense.Id,
            await DecryptAsync(drivingLicense.EncryptedLicenseNumber),
            drivingLicense.FrontSideImageUrl,
            drivingLicense.BackSideImageUrl
        );
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

using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Core.Results;
using Account.Domain.Aggregates.DrivingLicenseAggregate;
using Account.Domain.Aggregates.NationalIdentityAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using MediatR;
using Core.Dtos;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetPaginatedMechanicUser;

public class GetPaginatedMechanicUserQueryHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetPaginatedMechanicUserQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ApplicationService _appService = service;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<Result> Handle(GetPaginatedMechanicUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (totalCount, totalPages, users) = await _unitOfWork.BaseUsers.GetPaginatedMechanicUser(request.PageNumber, request.PageSize);

            if (users is null)
            {
                return Result.Failure("Regular user does not have any record", ResponseStatus.NotFound);
            }

            var userResponseDto = await ToMechanicUserResponseDto(users, cancellationToken);

            var response = new PaginatedResponseDto<MechanicUserResponseDto>(request.PageNumber, request.PageSize, totalCount, totalPages, userResponseDto);

            return Result.Success(response, ResponseStatus.Ok);
        }
        catch (Exception ex) 
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<IEnumerable<MechanicUserResponseDto>> ToMechanicUserResponseDto(IEnumerable<MechanicUser> users, CancellationToken cancellationToken)
    {
        if (users is null)
        {
            return [];
        }

        var responses = new List<MechanicUserResponseDto>();

        foreach (var user in users) 
        {
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

            responses.Add(new MechanicUserResponseDto(
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
                nationalIdentity,
                user.Rating,
                user.CreatedAtUtc, 
                user.TotalEntireJob,
                user.TotalCancelledJob,
                user.TotalEntireJobBothCompleteIncomplete,
                user.TotalCompletedJob,
                user.IsVerified,
                user.IsActive
            )); 
        }

        return responses; 
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

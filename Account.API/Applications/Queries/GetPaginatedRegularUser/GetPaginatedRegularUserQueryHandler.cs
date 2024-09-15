using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services; 
using Account.API.Extensions;
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR;
using Core.Dtos;
using Core.Services.Interfaces;

namespace Account.API.Applications.Queries.GetPaginatedRegularUser;

public class GetPaginatedRegularUserQueryHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService kmsClient,
    ApplicationService service,
    ICacheService cacheService) : IRequestHandler<GetPaginatedRegularUserQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ApplicationService _appService = service;
    public async Task<Result> Handle(GetPaginatedRegularUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (totalCount, totalPages, users) = await _unitOfWork.BaseUsers.GetPaginatedRegularUser(request.PageNumber, request.PageSize);

            if (users is null)
            {
                return Result.Failure("Regular user data is empty", ResponseStatus.NotFound);
            }

            var userResponseDto = await ToRegularUserResponseDto(users);

            var response = new PaginatedResponseDto<RegularUserResponseDto>(request.PageNumber, request.PageSize, totalCount, totalPages, userResponseDto);

            return Result.Success(response, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<IEnumerable<RegularUserResponseDto>> ToRegularUserResponseDto(IEnumerable<RegularUser> users)
    { 
        var responses = new List<RegularUserResponseDto>();
        foreach (var user in users)
        {
            var decryptedEmail = await DecryptNullableAsync(user.EncryptedEmail);
            var decryptedPhoneNumber = await DecryptNullableAsync(user.EncryptedPhoneNumber);
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
                user.Address.Country);

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

            responses.Add(new RegularUserResponseDto(
                    user.Id, 
                    decryptedEmail,
                    decryptedPhoneNumber,
                    user.TimeZoneId,
                    address,
                    user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
                    user.ReferralProgram.ToReferralProgramResponseDto(),
                    user.PersonalInfo.ToPersonalInfoResponseDto(user.TimeZoneId),
                    fleets));
        }
        return responses;
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

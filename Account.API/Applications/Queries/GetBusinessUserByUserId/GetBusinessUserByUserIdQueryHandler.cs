using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.API.Utilities;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public class GetBusinessUserByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<InMemoryDatabaseOption> cacheOption) : IRequestHandler<GetBusinessUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;  
    private readonly ICacheService _cacheService = cacheService; 
    private readonly IOptionsMonitor<InMemoryDatabaseOption> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetBusinessUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cacheService.GetAsync<MechanicUserResponseDto>($"{CacheKey.BusinessUserPrefix}#{request.Id}"); 
            if (result is not null)
            {
                return Result.Success(result, ResponseStatus.Ok);
            }
             
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id); 
            if (user is null)
            {
                return Result.Failure($"Business user '{request.Id}' is not found", ResponseStatus.NotFound);
            }

            var decryptedEmail = await DecryptAsync(user.EncryptedEmail);
            var decryptedPhoneNumber = await DecryptAsync(user.EncryptedPhoneNumber);
            var decryptedContactPerson = await DecryptAsync(user.EncryptedContactPerson);
            var decryptedTaxId = await DecryptNullableAsync(user.EncryptedTaxId);
            var decryptedAddressLine1 = await DecryptAsync(user.Address.EncryptedAddressLine1);
            var decryptedAddressLine2 = await DecryptNullableAsync(user.Address.EncryptedAddressLine2);
            var decryptedAddressLine3 = await DecryptNullableAsync(user.Address.EncryptedAddressLine3);

            var address = new AddressResponseDto(
                    // decrypted from existing entity
                    decryptedAddressLine1,
                    // decrypted from existing entity
                    decryptedAddressLine2,
                    // decrypted from existing entity
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
                    var decryptedStaffPhoneNumber = await DecryptAsync(staff.EncryptedPhoneNumber); 
                    var decryptedStaffEmail = await DecryptAsync(staff.EncryptedEmail);

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

                    staffResponses.Add(new StaffResponseDto( 
                            staff.Id,
                            staff.Username,
                            decryptedStaffEmail,
                            decryptedStaffPhoneNumber,
                            staff.Name,
                            staffAddress,
                            staff.TimeZoneId
                        ));
                }
            }

            var userDto = new BusinessUserResponseDto(
                user.Id,
                user.Username,
                decryptedEmail,
                decryptedPhoneNumber,
                user.TimeZoneId,
                address,
                user.BusinessName,
                decryptedContactPerson,
                decryptedTaxId,
                user.WebsiteUrl,
                user.Description,
                user.LoyaltyProgram.ToLoyaltyProgramResponseDto(),
                businessLicenseResponses,
                staffResponses);

            await _cacheService
               .SetAsync($"{CacheKey.BusinessUserPrefix}#{request.Id}", userDto, TimeSpan.FromSeconds(_cacheOptions.CurrentValue.CacheLifeTime));

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

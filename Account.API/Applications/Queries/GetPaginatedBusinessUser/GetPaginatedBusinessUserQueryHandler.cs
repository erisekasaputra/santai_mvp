using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Account.API.Extensions;
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedBusinessUser;

public class GetPaginatedBusinessUserQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetPaginatedBusinessUserQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<Result> Handle(GetPaginatedBusinessUserQuery request, CancellationToken cancellationToken)
    {
        try
        {  
            (int totalCount, int totalPages, IEnumerable<BusinessUser> businessUsers) = await _unitOfWork.BaseUsers.GetPaginatedBusinessUser(request.PageNumber, request.PageSize);

            if (businessUsers is null)
            {
                return Result.Failure($"Business user does not have any record", ResponseStatus.NotFound);
            }

            var businessUserResponses = new List<BusinessUserResponseDto>();

            foreach (var user in businessUsers) 
            {
                var decryptedEmail = await DecryptNullableAsync(user.EncryptedEmail);
                var decryptedPhoneNumber = await DecryptNullableAsync(user.EncryptedPhoneNumber);
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

                        staffResponses.Add(new StaffResponseDto(
                                staff.Id, 
                                decryptedStaffEmail,
                                decryptedStaffPhoneNumber,
                                staff.Name,
                                staffAddress,
                                staff.TimeZoneId
                            ));
                    }
                }

                businessUserResponses.Add(new BusinessUserResponseDto(
                    user.Id, 
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
                    staffResponses)); 
            }

            var paginatedResponse = new PaginatedResponseDto<BusinessUserResponseDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages,
                businessUserResponses);

            return Result.Success(paginatedResponse);
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

using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedStaffByUserId;

public class GetPaginatedStaffByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetPaginatedStaffByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<Result> Handle(GetPaginatedStaffByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            (int totalCount, int totalPages, IEnumerable<Staff> staffs) = await _unitOfWork.Staffs.GetPaginatedStaffByUserIdAsync(
                request.UserId, 
                request.PageNumber, 
                request.PageSize);

            if (staffs is null)
            {
                return Result.Failure($"Business user does not have any staff record", ResponseStatus.NotFound);
            }

            var staffResponses = new List<StaffResponseDto>();

            foreach (var staff in staffs)
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

            var paginatedResponse = new PaginatedResponseDto<StaffResponseDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages,
                staffResponses);

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

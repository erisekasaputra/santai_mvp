using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;

namespace Account.API.Applications.Queries.GetStaffByUserIdAndStaffId;

public class GetStaffByUserIdAndStaffIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetStaffByUserIdAndStaffIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetStaffByUserIdAndStaffIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _unitOfWork.Staffs.GetByBusinessUserIdAndStaffIdAsync(request.UserId, request.StaffId);

            if (staff is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            var decryptedEmail = await DecryptNullableAsync(staff.EncryptedEmail);
            var decryptedPhoneNumber = await DecryptNullableAsync(staff.EncryptedPhoneNumber);

            var decryptedAddressLine1 = await DecryptAsync(staff.Address.EncryptedAddressLine1);
            var decryptedAddressLine2 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine2);
            var decryptedAddressLine3 = await DecryptNullableAsync(staff.Address.EncryptedAddressLine3);


            var addressDto = new AddressResponseDto(
                decryptedAddressLine1,
                decryptedAddressLine2,
                decryptedAddressLine3,
                staff.Address.City,
                staff.Address.State,
                staff.Address.PostalCode,
                staff.Address.Country);

            var staffDto = new StaffResponseDto(
                    staff.Id, 
                    decryptedEmail,
                    decryptedPhoneNumber,
                    staff.Name,
                    addressDto,
                    staff.TimeZoneId );

            return Result.Success(staffDto, ResponseStatus.Ok); 
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
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

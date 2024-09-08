using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Account.API.Extensions;
using Core.Results;
using Core.Messages;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public class GetRegularUserByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetRegularUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;

    public async Task<Result> Handle(GetRegularUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        { 
            var user = await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            var userDto = await ToRegularUserResponseDto(user);
             
            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private async Task<RegularUserResponseDto> ToRegularUserResponseDto(RegularUser user)
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

        return new RegularUserResponseDto(
                user.Id,  
                decryptedEmail,
                decryptedPhoneNumber,
                user.TimeZoneId,
                address,
                user.PersonalInfo.ToPersonalInfoResponseDto(user.TimeZoneId));
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

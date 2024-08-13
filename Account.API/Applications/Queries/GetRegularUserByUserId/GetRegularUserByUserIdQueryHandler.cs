using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.API.Services;
using Account.API.Utilities;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public class GetRegularUserByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetRegularUserByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(GetRegularUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cacheService.GetAsync<RegularUserResponseDto>($"{CacheKey.RegularUserPrefix}#{request.UserId}");
            if (result is not null)
            {
                return Result.Success(result, ResponseStatus.Ok);
            }


            var user = await _unitOfWork.Users.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            var userDto = await ToRegularUserResponseDto(user);

            await _cacheService
              .SetAsync($"{CacheKey.RegularUserPrefix}#{request.UserId}", userDto, TimeSpan.FromSeconds(10));

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
        var decryptedEmail = await DecryptAsync(user.EncryptedEmail);
        var decryptedPhoneNumber = await DecryptAsync(user.EncryptedPhoneNumber);
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
                user.Username,
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

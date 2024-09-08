using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;

namespace Account.API.Applications.Queries.GetPhoneNumberByUserId;

public class GetPhoneNumberByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetPhoneNumberByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetPhoneNumberByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var phoneNumber = await _unitOfWork.BaseUsers.GetPhoneNumberById(request.UserId);

            if (phoneNumber is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(new
            {
                request.UserId,
                PhoneNumber = await DecryptAsync(phoneNumber),
            }, ResponseStatus.Ok);
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

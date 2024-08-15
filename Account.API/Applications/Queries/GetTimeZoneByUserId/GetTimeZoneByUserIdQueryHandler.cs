using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Infrastructures;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Queries.GetTimeZoneByUserId;

public class GetTimeZoneByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<InMemoryDatabaseOption> cacheOption) : IRequestHandler<GetTimeZoneByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<InMemoryDatabaseOption> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetTimeZoneByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeZoneId = await _unitOfWork.Users.GetTimeZoneById(request.UserId);

            if (timeZoneId is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            } 

            return Result.Success(new 
            {
                request.UserId,
                TimeZoneId = timeZoneId,
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

using Account.API.Infrastructures;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Queries.GetTimeZoneByStaffId;


public class GetTimeZoneByStaffIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<InMemoryDatabaseOption> cacheOption) : IRequestHandler<GetTimeZoneByStaffIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<InMemoryDatabaseOption> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetTimeZoneByStaffIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeZoneId = await _unitOfWork.Staffs.GetTimeZoneByIdAsync(request.StaffId);

            if (timeZoneId is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(new
            { 
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


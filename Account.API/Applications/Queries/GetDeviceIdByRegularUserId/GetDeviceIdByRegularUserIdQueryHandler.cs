using Account.API.Infrastructures;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Queries.GetDeviceIdByRegularUserId;


public class GetDeviceIdByRegularUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<InMemoryDatabaseOption> cacheOption) : IRequestHandler<GetDeviceIdByRegularUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<InMemoryDatabaseOption> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetDeviceIdByRegularUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var deviceId = await _unitOfWork.Users.GetDeviceIdByRegularUserId(request.UserId);

            if (deviceId is null)
            {
                return Result.Failure($"Staff '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(new
            {
                request.UserId,
                DeviceId = deviceId
            }, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
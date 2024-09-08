using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;

namespace Account.API.Applications.Queries.GetDeviceIdByMechanicUserId;

public class GetDeviceIdByMechanicUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetDeviceIdByMechanicUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetDeviceIdByMechanicUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var deviceId = await _unitOfWork.BaseUsers.GetDeviceIdByMechanicUserId(request.UserId);

            if (deviceId is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
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

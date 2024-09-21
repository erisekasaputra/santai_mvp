using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetDeviceIdByRegularUserId;


public class GetDeviceIdByRegularUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetDeviceIdByRegularUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetDeviceIdByRegularUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var deviceId = await _unitOfWork.BaseUsers.GetDeviceIdById(request.UserId); 
            if (deviceId is null)
            {
                return Result.Failure($"Staff '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(new
            {
                UserId = request.UserId,
                DeviceIds = deviceId
            }, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
using Core.Results;
using Core.Messages;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Configurations;

namespace Account.API.Applications.Queries.GetEmailByStaffId;

public class GetEmailByStaffIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService,
    IOptionsMonitor<CacheConfiguration> cacheOption) : IRequestHandler<GetEmailByStaffIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IOptionsMonitor<CacheConfiguration> _cacheOptions = cacheOption;
    public async Task<Result> Handle(GetEmailByStaffIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var email = await _unitOfWork.Staffs.GetEmailByIdAsync(request.StaffId);

            if (email is null)
            {
                return Result.Failure($"User '{request.StaffId}' not found", ResponseStatus.NotFound);
            }

            return Result.Success(new
            {
                request.StaffId,
                Email = await DecryptAsync(email),
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

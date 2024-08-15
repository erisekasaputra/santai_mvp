using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Infrastructures;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetDrivingLicenseByMechanicUserId;

public class GetDrivingLicenseByMechanicUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetDrivingLicenseByMechanicUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(GetDrivingLicenseByMechanicUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        { 
            var drivingLicense = await _unitOfWork.DrivingLicenses.GetOrderWithAcceptedByUserIdAsync(request.UserId);

            if (drivingLicense is null)
            {
                return Result.Failure($"Driving license not found", ResponseStatus.NotFound);
            }

            var decryptedLicenseNumber = await DecryptAsync(drivingLicense.EncryptedLicenseNumber);

            var drivingLicenseDto = new DrivingLicenseResponseDto(
                drivingLicense.Id,
                decryptedLicenseNumber,
                drivingLicense.FrontSideImageUrl,
                drivingLicense.BackSideImageUrl); 

            return Result.Success(drivingLicenseDto);
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

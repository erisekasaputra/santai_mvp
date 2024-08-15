using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Infrastructures;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetNationalIdentityByMechanicUserId;

public class GetNationalIdentityByMechanicUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetNationalIdentityByMechanicUserIdQuery, Result>
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(GetNationalIdentityByMechanicUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {  
            var nationalIdentity = await _unitOfWork.NationalIdentities.GetOrderWithAcceptedByUserIdAsync(request.UserId);

            if (nationalIdentity is null)
            {
                return Result.Failure($"Driving license not found", ResponseStatus.NotFound);
            }

            var decryptedIdentityNumber = await DecryptAsync(nationalIdentity.EncryptedIdentityNumber);

            var nationalIdentityDto = new NationalIdentityResponseDto(
                nationalIdentity.Id,
                decryptedIdentityNumber,
                nationalIdentity.FrontSideImageUrl,
                nationalIdentity.BackSideImageUrl); 

            return Result.Success(nationalIdentityDto);
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

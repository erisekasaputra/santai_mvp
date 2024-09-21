using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Applications.Services;
using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Dtos;
using Core.Services.Interfaces;
using Core.CustomMessages;

namespace Account.API.Applications.Queries.GetPaginatedBusinessLicenseByUserId;

public class GetPaginatedBusinessLicenseByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetPaginatedBusinessLicenseByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(GetPaginatedBusinessLicenseByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            (int totalCount, int totalPages, IEnumerable<BusinessLicense> businessLicenses) = await _unitOfWork.BusinessLicenses.GetPaginatedBusinessLicenseByUserId(request.UserId, request.PageNumber, request.PageSize);

            if (businessLicenses is null)
            {
                return Result.Failure($"Business user does not any business license record", ResponseStatus.NotFound);
            }

            var businessLicenseResponses = new List<BusinessLicenseResponseDto>();

            foreach (var businessLicense in businessLicenses)
            {
                var decryptedLicenseNumber = await DecryptAsync(businessLicense.EncryptedLicenseNumber); 

                businessLicenseResponses.Add(new BusinessLicenseResponseDto(
                    businessLicense.Id,
                    decryptedLicenseNumber,
                    businessLicense.Name,
                    businessLicense.Description));
            }

            var paginatedResponse = new PaginatedResponseDto<BusinessLicenseResponseDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages,
                businessLicenseResponses);

            return Result.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
    private async Task<string> DecryptAsync(string cipherText)
    {
        return await _kmsClient.DecryptAsync(cipherText);
    }

    private async Task<string?> DecryptNullableAsync(string? cipherText)
    {
        if (cipherText == null) { return null; }

        return await _kmsClient.DecryptAsync(cipherText);
    }
}

using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.Infrastructures;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetPaginatedMechanicCertificationByUserId;

public class GetPaginatedMechanicCertificationByUserIdQueryHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    ICacheService cacheService) : IRequestHandler<GetPaginatedMechanicCertificationByUserIdQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(GetPaginatedMechanicCertificationByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound);
            }

            (int totalCount, int totalPages, IEnumerable<Certification> certifications) = await _unitOfWork.Certifications.GetPaginatedCertificationByUserId(
                request.UserId,
                request.PageNumber,
                request.PageSize);

            if (certifications is null)
            {
                return Result.Failure($"Mechanic user does not have any certification record", ResponseStatus.NotFound);
            }

            var paginatedResponse = new PaginatedItemReponseDto<CertificationResponseDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                totalPages,
                certifications.ToCertificationResponseDtos(mechanicUser.TimeZoneId)!);

            return Result.Success(paginatedResponse, ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

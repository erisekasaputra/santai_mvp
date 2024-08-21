using Account.API.Applications.Dtos.ResponseDtos;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessLicenseByUserId;

public class CreateBusinessLicenseByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<CreateBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _appService = service; 
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(CreateBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var entity = await _unitOfWork.BaseUsers.GetAnyByIdAsync(request.BusinessUserId);
            if (entity is false)
            {
                return Result.Failure($"Business user not found", ResponseStatus.NotFound)
                    .WithError(new ("BusinessUser.Id", "Business user id not found"));
            }

            var hashedLicenseNumber = await _hashClient.Hash(request.LicenseNumber);
            var encrypedLicenseNumber = await _kmsClient.EncryptAsync(request.LicenseNumber);

            var conclict = await _unitOfWork.BusinessLicenses.GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(hashedLicenseNumber);
            if (conclict is not null)
            {
                var errors = new List<ErrorDetail>();
                if (conclict!.HashedLicenseNumber == hashedLicenseNumber)
                {
                    errors.Add(new ("BusinessLicense.LicenseNumber", "Can not have multiple license with 'Accepted' status"));
                }

                return Result.Failure("There is a conflict", ResponseStatus.BadRequest).WithErrors(errors);
            }

            var businessLicense = new BusinessLicense(
                request.BusinessUserId,
                hashedLicenseNumber,
                encrypedLicenseNumber,
                request.Name,
                request.Description);

            var createdBusinessLicense = await _unitOfWork.BusinessLicenses.CreateAsync(businessLicense);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(ToBusinessLicenseResponseDto(createdBusinessLicense, request), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _appService.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }

    private static BusinessLicenseResponseDto ToBusinessLicenseResponseDto(
        BusinessLicense businessLicense,
        CreateBusinessLicenseByUserIdCommand request)
    {
        return new BusinessLicenseResponseDto(
            businessLicense.Id,
            request.LicenseNumber,
            businessLicense.Name,
            businessLicense.Description);
    } 
}

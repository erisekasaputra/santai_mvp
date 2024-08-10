using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Mapper;
using Account.API.SeedWork;
using Account.Domain.Aggregates.BusinessLicenseAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Applications.Commands.CreateBusinessLicenseByUserId;

public class CreateBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<CreateBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _appService = service;

    public async Task<Result> Handle(CreateBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            var license = request.Request; 
            var entity = await _unitOfWork.Users.GetAnyByIdAsync(request.BusinessUserId); 
            if (entity is false)
            { 
                return Result.Failure($"Business user '{request.BusinessUserId}' not found", ResponseStatus.NotFound);
            } 

            var licenseConflict = await _unitOfWork.BusinessLicenses.GetAcceptedStatusByLicenseNumberAsNoTrackingAsync(license.LicenseNumber); 
            if (licenseConflict is not null)
            {
                var errorDetails = new List<ErrorDetail>();  
                if (licenseConflict!.LicenseNumber == license.LicenseNumber)
                {
                    errorDetails.Add(new ErrorDetail("LicenseNumber", $"Can not have multiple license numbers {license.LicenseNumber} with 'Accepted' status"));
                }

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a conflict",
                    _ => $"There are {errorDetails.Count} conflicts"
                };
                 
                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }

            var businessLicense = new BusinessLicense(request.BusinessUserId, license.LicenseNumber, license.Name, license.Description); 
            var createdBusinessLicense = await _unitOfWork.BusinessLicenses.CreateAsync(businessLicense); 
            await _unitOfWork.SaveChangesAsync(cancellationToken); 
            return Result.Success(createdBusinessLicense.ToBusinessLicenseResponseDto(), ResponseStatus.Created);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        { 
            _appService.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

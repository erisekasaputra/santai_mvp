using Account.API.Applications.Services;
using Account.API.Extensions;
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
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.BusinessUserId);

            if (user is null)
            { 
                return Result.Failure($"Business user '{request.BusinessUserId}' not found", ResponseStatus.NotFound);
            }


            var licenseConflict = await _unitOfWork.BusinessLicenses.GetAcceptedByNumberAsNoTrackAsync(license.LicenseNumber);

            if (licenseConflict is not null)
            {
                var errorDetails = new List<ErrorDetail>();

                if (licenseConflict!.LicenseNumber.Clean() == license.LicenseNumber.Clean())
                {
                    errorDetails.Add(new ErrorDetail("LicenseNumber", $"Can not have multiple license numbers with accepted status"));
                }

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a conflict",
                    _ => $"There are {errorDetails.Count} conflicts"
                };
                 
                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }


            (BusinessLicense? newBusinessLicense, string? errorParameter, string? errorMessage) = user.AddBusinessLicenses(license.LicenseNumber, license.Name, license.Description);

            if (newBusinessLicense is null && errorParameter is not null)
            {
                return Result.Failure("There is a conflict", ResponseStatus.BadRequest)
                    .WithError(new ErrorDetail(errorParameter, errorMessage ?? string.Empty)); 
            }

            if (newBusinessLicense is not null)
            {
                _unitOfWork.AttachEntity(newBusinessLicense);
                _unitOfWork.SetEntityState(newBusinessLicense, EntityState.Added);
            }  
             

            _unitOfWork.Users.UpdateUser(user); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        { 
            _appService.Logger.LogError(ex.Message);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);

        }
        catch (Exception ex)
        { 
            _appService.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

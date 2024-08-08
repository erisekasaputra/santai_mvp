using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork; 
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR; 

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
                return Result.Failure($"Business user with id {request.BusinessUserId} is not found", ResponseStatus.NotFound);
            }

            var licenseConflicts = await _unitOfWork.BusinessLicenses.GetByNumberAsNoTrackAsync([license.LicenseNumber]);
             
            if (licenseConflicts is not null && licenseConflicts.Any())
            {
                var licenseConflict = licenseConflicts.FirstOrDefault();
                var errorDetails = new List<ErrorDetail>();

                if (licenseConflict!.LicenseNumber.Clean() == license.LicenseNumber.Clean())
                {
                    errorDetails.Add(new ErrorDetail("LicenseNumber", $"License number {license.LicenseNumber} already registered"));
                } 

                var message = errorDetails.Count switch
                {
                    <= 1 => "There is a data conflict",
                    _ => $"There are {errorDetails.Count} data conflicts"
                };

                return Result.Failure(message, ResponseStatus.BadRequest).WithErrors(errorDetails);
            }

            (string? errorParameter, string? errorMessage) = user.AddBusinessLicenses(license.LicenseNumber, license.Name, license.Description);

            if (errorParameter is not null)
            {
                return Result.Failure("There is a conflict while save the business license into a business user data", ResponseStatus.BadRequest)
                    .WithError(new ErrorDetail(errorParameter, errorMessage ?? string.Empty));
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
            return Result.Failure("An error has occurred while create staff for a business user", ResponseStatus.InternalServerError);
        }
    }
}

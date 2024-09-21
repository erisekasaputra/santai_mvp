using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.BusinessUserCommand.RejectBusinessLicenseByUserId;

public class RejectBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<RejectBusinessLicenseByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(RejectBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var license = await _unitOfWork.BusinessLicenses.GetByBusinessUserIdAndBusinessLicenseIdAsync(request.BusinessUserId, request.BusinessLicenseId);
            if (license is null)
            {
                return Result.Failure($"Business license not found", ResponseStatus.NotFound)
                    .WithError(new("BusinessLicense.Id", "Business license not found"));
            }

            license.RejectDocument();
            
            _unitOfWork.BusinessLicenses.Update(license);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

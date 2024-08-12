using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

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
                return Result.Failure($"Busines user id '{request.BusinessUserId}' and business license '{request.BusinessLicenseId}' did not match any record", ResponseStatus.NotFound);
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
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

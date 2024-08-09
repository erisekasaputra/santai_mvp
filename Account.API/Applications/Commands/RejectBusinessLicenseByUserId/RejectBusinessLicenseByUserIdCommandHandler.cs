using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RejectBusinessLicenseByUserId;

public class RejectBusinessLicenseByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<RejectBusinessLicenseByUserIdCommand, Result>
{  
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(RejectBusinessLicenseByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.BusinessUserId);

            if (user is null)
            {
                return Result.Failure($"Business user with id {request.BusinessUserId} not found", ResponseStatus.NotFound);
            }

            var license = await _unitOfWork.BusinessLicenses.GetByIdAsync(request.BusinessLicenseId);

            if (license is null)
            {
                return Result.Failure($"Business license with id {request.BusinessLicenseId} not found", ResponseStatus.NotFound);
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
            _service.Logger.LogError(ex.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

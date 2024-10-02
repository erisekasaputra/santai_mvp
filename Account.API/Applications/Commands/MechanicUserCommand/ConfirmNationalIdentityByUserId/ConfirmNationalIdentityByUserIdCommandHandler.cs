using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR; 
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.MechanicUserCommand.ConfirmNationalIdentityByUserId;

public class ConfirmNationalIdentityByUserIdCommandHandler : IRequestHandler<ConfirmNationalIdentityByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly ApplicationService _service; 

    public ConfirmNationalIdentityByUserIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ApplicationService service )
    {
        _unitOfWork = unitOfWork; 
        _service = service; 
    }

    public async Task<Result> Handle(ConfirmNationalIdentityByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var license = await _unitOfWork.NationalIdentities.GetByUserIdAndIdAsync(request.UserId, request.NationalIdentityId);

            if (license is null)
            {
                return Result.Failure($"National identity not found", ResponseStatus.NotFound);
            }

            var accepted = await _unitOfWork.NationalIdentities.GetAcceptedByUserIdAsync(request.UserId);

            if (accepted is not null && accepted.Id == request.NationalIdentityId)
            {
                return Result.Failure($"National identity already accepted", ResponseStatus.Conflict);
            }

            if (accepted is not null)
            {
                return Result.Failure("Can only have one 'Accepted' national identity for a user", ResponseStatus.Conflict);
            }

            license.VerifyDocument();

            _unitOfWork.NationalIdentities.Update(license);

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

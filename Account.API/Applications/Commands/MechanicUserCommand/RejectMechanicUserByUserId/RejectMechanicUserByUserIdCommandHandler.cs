using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages; 

namespace Account.API.Applications.Commands.MechanicUserCommand.RejectMechanicUserByUserId;

public class RejectMechanicUserByUserIdCommandHandler : IRequestHandler<RejectMechanicUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public RejectMechanicUserByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ApplicationService service,
        IEncryptionService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }

    public async Task<Result> Handle(RejectMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound);
            }

            mechanicUser.RejectDocument();

            _unitOfWork.BaseUsers.Update(mechanicUser);

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

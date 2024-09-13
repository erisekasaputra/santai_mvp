using Account.API.Applications.Services;
using Account.Domain.SeedWork;
using Core.Exceptions;
using Core.Messages;
using Core.Results;
using Core.Services.Interfaces;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;

public class ActivateMechanicStatusByUserIdCommandHandler : IRequestHandler<ActivateMechanicStatusByUserIdCommand, Result>

{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public ActivateMechanicStatusByUserIdCommandHandler(
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
    public async Task<Result> Handle(ActivateMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.MechanicId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.Activate();

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

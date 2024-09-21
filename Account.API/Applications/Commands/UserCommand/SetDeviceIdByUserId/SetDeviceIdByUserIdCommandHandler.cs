using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.UserCommand.SetDeviceIdByUserId;

public class SetDeviceIdByUserIdCommandHandler : IRequestHandler<SetDeviceIdByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationService _service;
    private readonly IEncryptionService _kmsClient;
    private readonly IHashService _hashService;

    public SetDeviceIdByUserIdCommandHandler(
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

    public async Task<Result> Handle(SetDeviceIdByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetByIdAsync(request.UserId); 
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound)
                     .WithError(new("User.Id", "User not found"));
            }

            user.AddDeviceId(request.DeviceId);
            _unitOfWork.BaseUsers.Update(user);
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

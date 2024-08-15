using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using Microsoft.Extensions.Options;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;

public class DeleteMechanicUserByUserIdCommandHandler : IRequestHandler<DeleteMechanicUserByUserIdCommand, Result>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptionsMonitor<ReferralProgramOption> _referralOptions;
    private readonly ApplicationService _service;
    private readonly IKeyManagementService _kmsClient;
    private readonly IHashService _hashService;

    public DeleteMechanicUserByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<ReferralProgramOption> referralOptions,
        ApplicationService service,
        IKeyManagementService kmsClient,
        IHashService hashService)
    {
        _unitOfWork = unitOfWork;
        _referralOptions = referralOptions;
        _service = service;
        _kmsClient = kmsClient;
        _hashService = hashService;
    }


    public async Task<Result> Handle(DeleteMechanicUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mechanicUser = await _unitOfWork.Users.GetMechanicUserByIdAsync(request.UserId);

            if (mechanicUser is null)
            {
                return Result.Failure($"Mechanic user not found", ResponseStatus.NotFound)
                     .WithError(new("MechanicUser.Id", "Mechanic user not found"));
            }

            mechanicUser.Delete();

            _unitOfWork.Users.Delete(mechanicUser);

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

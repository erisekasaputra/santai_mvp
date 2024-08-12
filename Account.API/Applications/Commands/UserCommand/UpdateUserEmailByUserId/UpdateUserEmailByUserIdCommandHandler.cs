using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.UpdateUserEmailByUserId;

public class UpdateUserEmailByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateUserEmailByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateUserEmailByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"User with id {request.Id} not found", ResponseStatus.NotFound);
            }

            var hashedEmail = await _hashClient.Hash(request.Email);
            var encryptedEmail = await _kmsClient.EncryptAsync(request.Email);

            var conflict = await _unitOfWork.Users.GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(request.Id, (IdentityParameter.Email, hashedEmail));
            if (conflict)
            {
                return Result.Failure($"Email already registered", ResponseStatus.Conflict)
                    .WithError(new ErrorDetail("User.Email", "Email already registered"));
            }

            user.UpdateEmail(hashedEmail, encryptedEmail);

            _unitOfWork.Users.Update(user);
            
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

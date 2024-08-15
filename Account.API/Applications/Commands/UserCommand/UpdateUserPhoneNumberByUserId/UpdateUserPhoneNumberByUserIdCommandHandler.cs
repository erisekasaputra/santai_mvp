using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.UpdateUserPhoneNumberByUserId;

public class UpdateUserPhoneNumberByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IKeyManagementService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateUserPhoneNumberByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateUserPhoneNumberByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
            
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound)
                    .WithError(new("User.Id", "User not found"));
            }

            var hashedPhoneNumber = await _hashClient.Hash(request.PhoneNumber);
            var encryptedPhoneNumber = await _kmsClient.EncryptAsync(request.PhoneNumber);

            var conflict = await _unitOfWork.Users
                .GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(request.Id, (IdentityParameter.PhoneNumber, hashedPhoneNumber));
            
            if (conflict)
            {
                return Result.Failure($"Phone number already registered", ResponseStatus.Conflict)
                    .WithError(new ErrorDetail("Staff.PhoneNumber", "Phone number already registered"));
            }

            user.UpdatePhoneNumber(hashedPhoneNumber, encryptedPhoneNumber);
            
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

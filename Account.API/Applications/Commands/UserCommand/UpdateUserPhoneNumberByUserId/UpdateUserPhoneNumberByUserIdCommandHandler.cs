using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.UserCommand.UpdateUserPhoneNumberByUserId;

public class UpdateUserPhoneNumberByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient,
    IHashService hashService) : IRequestHandler<UpdateUserPhoneNumberByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IHashService _hashClient = hashService;

    public async Task<Result> Handle(UpdateUserPhoneNumberByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetByIdAsync(request.Id);
            
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }

            var hashedPhoneNumber = await _hashClient.Hash(request.PhoneNumber);
            var encryptedPhoneNumber = await _kmsClient.EncryptAsync(request.PhoneNumber);

            var conflict = await _unitOfWork.BaseUsers
                .GetAnyByIdentitiesExcludingIdAsNoTrackingAsync(request.Id, (IdentityParameter.PhoneNumber, hashedPhoneNumber));
            
            if (conflict)
            {
                return Result.Failure($"Phone number already registered", ResponseStatus.Conflict);
            }

            user.UpdatePhoneNumber(hashedPhoneNumber, encryptedPhoneNumber);
            
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

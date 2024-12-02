using Account.API.Applications.Services; 
using Core.Results;
using Account.Domain.SeedWork; 
using MediatR;
using Core.Services.Interfaces;
using Core.Exceptions;
using Core.CustomMessages; 

namespace Account.API.Applications.Commands.RegularUserCommand.UpdateProfilPictureRegularUser;

public class UpdateProfilePictureRegularUserCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service,
    IEncryptionService kmsClient) : IRequestHandler<UpdateProfilePictureRegularUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    private readonly IEncryptionService _kmsClient = kmsClient;

    public async Task<Result> Handle(UpdateProfilePictureRegularUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId);
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound);
            }
             

            user.UpdateProfilePicture(request.Path);

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

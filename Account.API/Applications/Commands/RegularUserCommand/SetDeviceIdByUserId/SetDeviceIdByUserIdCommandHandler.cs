using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.SetDeviceIdByUserId;

public class SetDeviceIdByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<SetDeviceIdByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(SetDeviceIdByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId);
            if (user is null)
            {
                return Result.Failure($"Regular user not found", ResponseStatus.NotFound)
                    .WithError(new("RegularUser.Id", "User not found"));
            }

            user.SetDeviceId(request.DeviceId);
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

using Account.API.Extensions;
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
            var user = await _unitOfWork.Users.GetRegularUserByIdAsync(request.UserId);
            if (user is null)
            {
                return Result.Failure($"User '{request.UserId}' not found", ResponseStatus.NotFound);
            }

            user.SetDeviceId(request.DeviceId);
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

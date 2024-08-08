using Account.API.Applications.Services;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateUserPhoneNumberByUserId;

public class UpdateUserPhoneNumberByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<UpdateUserPhoneNumberByUserIdCommand, Result>
{
    private readonly IUnitOfWork _UnitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(UpdateUserPhoneNumberByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _UnitOfWork.Users.GetUserByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"User with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            user.UpdatePhoneNumber(request.Request.PhoneNumber);
  
            _UnitOfWork.Users.UpdateUser(user);
            
            await _UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _service.Logger.LogError(ex.Message);
            return Result.Failure("An error has occurred while updating user phone number", ResponseStatus.InternalServerError);
        }
    }
}

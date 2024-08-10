using Account.API.Applications.Services;
using Account.API.Extensions;
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
            var user = await _UnitOfWork.Users.GetByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"User with id {request.Id} not found", ResponseStatus.NotFound);
            }

            user.UpdatePhoneNumber(request.Request.PhoneNumber);
  
            _UnitOfWork.Users.Update(user);
            
            await _UnitOfWork.SaveChangesAsync(cancellationToken);

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

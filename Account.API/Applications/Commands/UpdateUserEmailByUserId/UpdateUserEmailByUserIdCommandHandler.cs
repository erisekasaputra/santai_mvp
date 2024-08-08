using Account.API.Applications.Services;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateUserEmailByUserId;

public class UpdateUserEmailByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<UpdateUserEmailByUserIdCommand, Result>
{
    private readonly IUnitOfWork _UnitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(UpdateUserEmailByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _UnitOfWork.Users.GetUserByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"User with id {request.Id} is not found", ResponseStatus.NotFound);
            }

            user.UpdateEmail(request.Request.Email); 

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
            return Result.Failure("An error has occurred while updating user email", ResponseStatus.InternalServerError);
        }
    }
}

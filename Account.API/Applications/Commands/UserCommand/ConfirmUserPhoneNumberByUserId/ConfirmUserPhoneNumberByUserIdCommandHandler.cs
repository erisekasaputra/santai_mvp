using Account.API.Extensions;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;

public class ConfirmUserPhoneNumberByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<ConfirmUserPhoneNumberByUserIdCommand, Result>
{
    private readonly IUnitOfWork _UnitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(ConfirmUserPhoneNumberByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _UnitOfWork.Users.GetByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"User '{request.Id}' not found", ResponseStatus.NotFound);
            }

            user.VerifyPhoneNumber();

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

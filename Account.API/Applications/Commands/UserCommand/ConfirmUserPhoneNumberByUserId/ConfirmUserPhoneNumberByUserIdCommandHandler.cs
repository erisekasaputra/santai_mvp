using Account.API.Applications.Services;
using Core.Results;
using Core.Messages;
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
            var user = await _UnitOfWork.BaseUsers.GetByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound)
                    .WithError(new("User.Id", "User not found"));
            }

            user.VerifyPhoneNumber();

            _UnitOfWork.BaseUsers.Update(user);
            
            await _UnitOfWork.SaveChangesAsync(cancellationToken);
            
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

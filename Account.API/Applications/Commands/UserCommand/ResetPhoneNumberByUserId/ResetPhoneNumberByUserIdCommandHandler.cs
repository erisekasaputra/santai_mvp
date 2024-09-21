using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.UserCommand.ResetPhoneNumberByUserId;

public class ResetPhoneNumberByUserIdCommandHandler(
    IUnitOfWork unitOfWork,
    ApplicationService service) : IRequestHandler<ResetPhoneNumberByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;

    public async Task<Result> Handle(ResetPhoneNumberByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.BaseUsers.GetByIdAsync(request.Id);
            if (user is null)
            {
                return Result.Failure($"User not found", ResponseStatus.NotFound)
                    .WithError(new("User.Id", "User not found"));
            }

            user.ResetPhoneNumber();

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

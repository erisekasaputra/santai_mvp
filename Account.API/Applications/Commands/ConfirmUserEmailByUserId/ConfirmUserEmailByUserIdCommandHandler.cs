using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.SeedWork;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.ConfirmUserEmailByUserId;

public class ConfirmUserEmailByUserIdCommandHandler(IUnitOfWork unitOfWork, AppService service) : IRequestHandler<ConfirmUserEmailByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppService _service = service;

    public async Task<Result> Handle(ConfirmUserEmailByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);

            if (user is null)
            {
                return Result.Failure($"User '{request.Id}' not found", ResponseStatus.NotFound);
            }

            user.VerifyEmail();
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

using Account.API.Applications.Services;
using Core.Results;
using Account.Domain.SeedWork;
using MediatR;
using System.Data;
using Core.Exceptions;
using Core.CustomMessages;

namespace Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;

public class DeleteRegularUserByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<DeleteRegularUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(DeleteRegularUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var user = await _unitOfWork.BaseUsers.GetRegularUserByIdAsync(request.UserId);

            if (user is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken); 
                return Result.Failure($"Regular user not found", ResponseStatus.NotFound)
                    .WithError(new("RegularUser.Id", "User not found"));
            }

            await _unitOfWork.Fleets.DeleteByUserId(request.UserId);

            user.Delete();

            _unitOfWork.BaseUsers.Delete(user);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _service.Logger.LogError(ex, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

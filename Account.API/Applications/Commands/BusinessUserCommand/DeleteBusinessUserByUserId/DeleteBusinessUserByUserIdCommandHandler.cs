using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using MediatR;
using System.Data;

namespace Account.API.Applications.Commands.BusinessUserCommand.DeleteBusinessUserByUserId;

public class DeleteBusinessUserByUserIdCommandHandler(IUnitOfWork unitOfWork, ApplicationService service) : IRequestHandler<DeleteBusinessUserByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationService _service = service;
    public async Task<Result> Handle(DeleteBusinessUserByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var user = await _unitOfWork.Users.GetBusinessUserByIdAsync(request.Id);
            if (user is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure($"Business user {request.Id} is not found", ResponseStatus.NotFound)
                    .WithError(new("BusinessUser.Id", "Business user id not found"));
            }

            await _unitOfWork.Fleets.DeleteByUserId(request.Id);

            user.Delete();
             
            _unitOfWork.Users.Delete(user);
            
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
            _service.Logger.LogError(ex.Message, ex.InnerException?.Message);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

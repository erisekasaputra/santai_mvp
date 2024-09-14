using Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId; 
using Account.Domain.SeedWork;
using Core.Exceptions;
using Core.Messages;
using Core.Results; 
using Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;

namespace Account.API.Applications.Commands.MechanicUserCommand.DeactivateMechanicStatusByUserId;

public class DeactivateMechanicStatusByUserIdCommandHandler : IRequestHandler<DeactivateMechanicStatusByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<ActivateMechanicStatusByUserIdCommandHandler> _logger;

    public DeactivateMechanicStatusByUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ActivateMechanicStatusByUserIdCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _asyncRetryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>() 
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry on {retryCount} failed. Waiting {timeSpan} before try again. Error: {exception.Message}");
                });
    }
    public async Task<Result> Handle(DeactivateMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
                try
                {
                    var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.MechanicId); 
                    if (mechanic is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("Mechanic not found", ResponseStatus.NotFound);
                    }

                    if (!mechanic.IsVerified)
                    {
                        return Result.Failure("Mechanic verification document is still waiting for verification", ResponseStatus.BadRequest);
                    } 

                    var mechanicUser = await _unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdUnassignedOrderAsync(request.MechanicId);

                    if (mechanicUser is null)
                    { 
                        throw new DBConcurrencyException();
                    }

                    mechanicUser.Deactivate();

                    _unitOfWork.OrderTasks.UpdateMechanicTask(mechanicUser);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Success(null, ResponseStatus.NoContent);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            });

            return result;
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

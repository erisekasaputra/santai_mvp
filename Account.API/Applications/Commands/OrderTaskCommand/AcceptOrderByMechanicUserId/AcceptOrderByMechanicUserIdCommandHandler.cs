using Account.API.Applications.Services.Interfaces;
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
using System.Data.Common;

namespace Account.API.Applications.Commands.OrderTaskCommand.AcceptOrderByMechanicUserId;

public class AcceptOrderByMechanicUserIdCommandHandler : IRequestHandler<AcceptOrderByMechanicUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<AcceptOrderByMechanicUserIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;

    public AcceptOrderByMechanicUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AcceptOrderByMechanicUserIdCommandHandler> logger,
        IMechanicCache mechanicCache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mechanicCache = mechanicCache;
        _asyncRetryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>()
            .Or<DbException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    LoggerHelper.LogError(logger, exception);
                });
    }

    public async Task<Result> Handle(AcceptOrderByMechanicUserIdCommand request, CancellationToken cancellationToken)
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
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("Mechanic verification document is still waiting for verification", ResponseStatus.BadRequest);
                    }





                    var mechanicTask = await _unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdAsync(request.MechanicId);
                    if (mechanicTask is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
                    }

                    if (!mechanicTask.OrderId.HasValue)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("You dont have any waiting order", ResponseStatus.BadRequest);
                    }





                    // Retrieve the mechanic task
                    var orderWaitingMechanicAssign = await _unitOfWork.OrderTasks.GetOrderWaitingMechanicAssignByOrderIdAsync(mechanicTask.OrderId.Value);

                    if (orderWaitingMechanicAssign is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("Order not found", ResponseStatus.NotFound);
                    }
                    orderWaitingMechanicAssign.AcceptOrderByMechanic(mechanicTask.MechanicId); 

                    var orderWaitingMechanicConfirm = await _unitOfWork.OrderTasks.GetOrderWaitingMechanicConfirmByOrderIdAsync(mechanicTask.OrderId.Value);
                    if (orderWaitingMechanicConfirm is not null)
                    {
                        orderWaitingMechanicConfirm.SetDelete(mechanicTask.MechanicId);
                        _unitOfWork.OrderTasks.UpdateOrderConfirm(orderWaitingMechanicConfirm);
                    }  

                    _unitOfWork.OrderTasks.UpdateOrderAssign(orderWaitingMechanicAssign);   
                    await _unitOfWork.CommitTransactionAsync(); 
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

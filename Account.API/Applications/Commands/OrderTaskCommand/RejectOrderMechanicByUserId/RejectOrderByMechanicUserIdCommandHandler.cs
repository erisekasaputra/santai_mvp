using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.Common;

namespace Account.API.Applications.Commands.OrderTaskCommand.RejectOrderMechanicByUserId;

public class RejectOrderByMechanicUserIdCommandHandler : IRequestHandler<RejectOrderByMechanicUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<RejectOrderByMechanicUserIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;

    public RejectOrderByMechanicUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RejectOrderByMechanicUserIdCommandHandler> logger,
        IMechanicCache mechanicCache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mechanicCache = mechanicCache;
        _asyncRetryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>() 
            .Or<DbException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    LoggerHelper.LogError(logger, exception);
                });
    }


    public async Task<Result> Handle(RejectOrderByMechanicUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            {
                try
                {
                    (var isSuccess, var buyerId) = await _mechanicCache.RejectOrderByMechanic(request.OrderId.ToString(), request.MechanicId.ToString()); 
                    if (isSuccess)
                    {
                        return Result.Success(null, ResponseStatus.NoContent);
                    }

                    throw new InvalidOperationException();
                }
                catch (Exception)
                { 
                    throw;
                }
            });

            if (result.IsSuccess) 
            { 
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            else
            { 
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }

            return result;
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            LoggerHelper.LogError(_logger, ex);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

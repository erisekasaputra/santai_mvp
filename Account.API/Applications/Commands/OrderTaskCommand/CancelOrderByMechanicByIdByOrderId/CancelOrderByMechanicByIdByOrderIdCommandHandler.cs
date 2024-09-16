using Account.API.Applications.Services.Interfaces;
using Core.Results;
using MediatR;
using Polly.Retry;
using System.Data.Common;
using System.Data;
using Account.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Polly;
using Core.Utilities;
using Core.Exceptions;
using Core.Messages;

namespace Account.API.Applications.Commands.OrderTaskCommand.CancelOrderByMechanicByIdByOrderId;

public class CancelOrderByMechanicByIdByOrderIdCommandHandler : IRequestHandler<CancelOrderByMechanicByIdByOrderIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<CancelOrderByMechanicByIdByOrderIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;

    public CancelOrderByMechanicByIdByOrderIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelOrderByMechanicByIdByOrderIdCommandHandler> logger,
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

    public async Task<Result> Handle(CancelOrderByMechanicByIdByOrderIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
                try
                {
                    var result = await _mechanicCache.CancelOrderByMechanic(request.MechanicId.ToString(), request.OrderId.ToString());

                    if (result)
                    {
                        return Result.Success(null, ResponseStatus.NoContent);
                    }

                    throw new InvalidOperationException();
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

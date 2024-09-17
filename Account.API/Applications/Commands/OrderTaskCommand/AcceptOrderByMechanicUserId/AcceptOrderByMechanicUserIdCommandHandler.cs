using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Amazon.SecretsManager.Model.Internal.MarshallTransformations;
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
            .Handle<InvalidOperationException>() 
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    LoggerHelper.LogError(logger, exception);
                });
    }

    public async Task<Result> Handle(AcceptOrderByMechanicUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            { 
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
                try
                {
                    var result = await _mechanicCache.AcceptOrderByMechanic(request.OrderId.ToString(), request.MechanicId.ToString());

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

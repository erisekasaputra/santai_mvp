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

namespace Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;

public class ActivateMechanicStatusByUserIdCommandHandler : IRequestHandler<ActivateMechanicStatusByUserIdCommand, Result>

{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<ActivateMechanicStatusByUserIdCommandHandler> _logger;
    private readonly IMechanicCache _cache;

    public ActivateMechanicStatusByUserIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<ActivateMechanicStatusByUserIdCommandHandler> logger,
        IMechanicCache cache)
    {
        _cache = cache;
        _unitOfWork = unitOfWork; 
        _logger = logger; 
        _asyncRetryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>()
            .Or<DbException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), 
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogError("Retry on {retryCount} failed. Waiting {timeSpan} before try again. Error: {message}", retryCount, timeSpan, exception.Message);
                });
    }
    public async Task<Result> Handle(ActivateMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
        try
        {
            var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.MechanicId);
            
            if (mechanic is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Mechanic not found", ResponseStatus.NotFound);    
            }

            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            {
                //if (!mechanic.IsVerified)
                //{
                //    return Result.Failure("You account has not been verified", ResponseStatus.BadRequest);
                //}

                var result = await _cache.Activate(request.MechanicId.ToString());

                if (result)
                { 
                    return Result.Success(null, ResponseStatus.NoContent);
                }

                throw new InvalidOperationException();
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

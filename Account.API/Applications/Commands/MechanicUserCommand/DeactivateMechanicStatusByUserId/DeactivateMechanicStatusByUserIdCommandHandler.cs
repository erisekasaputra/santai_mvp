using Account.API.Applications.Commands.MechanicUserCommand.ActivateMechanicStatusByUserId;
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

namespace Account.API.Applications.Commands.MechanicUserCommand.DeactivateMechanicStatusByUserId;

public class DeactivateMechanicStatusByUserIdCommandHandler : IRequestHandler<DeactivateMechanicStatusByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<ActivateMechanicStatusByUserIdCommandHandler> _logger;
    private readonly IMechanicCache _cache;

    public DeactivateMechanicStatusByUserIdCommandHandler(
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
                    Console.WriteLine($"Retry on {retryCount} failed. Waiting {timeSpan} before try again. Error: {exception.Message}");
                });
    }
    public async Task<Result> Handle(DeactivateMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
                try
                {
                    var result = await _cache.Deactivate(request.MechanicId.ToString());

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

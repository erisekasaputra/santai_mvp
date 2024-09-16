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
            .WaitAndRetryAsync(2, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)), 
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry on {retryCount} failed. Waiting {timeSpan} before try again. Error: {exception.Message}");
                });
    }
    public async Task<Result> Handle(ActivateMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncRetryPolicy.ExecuteAsync<Result>(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
                try
                {
                    var result = await _cache.Activate(request.MechanicId.ToString());

                    if (result)
                    {
                        return Result.Success(null, ResponseStatus.NoContent);
                    }

                    throw new Exception();
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

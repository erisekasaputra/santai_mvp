using Account.API.Applications.Services.Interfaces;
using Account.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using Core.Utilities;
using MediatR;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.Common;

namespace Account.API.Applications.Commands.MechanicUserCommand.UnblockMechanicStatusByUserId;

public class UnblockMechanicStatusByUserIdCommandHandler : IRequestHandler<UnblockMechanicStatusByUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<UnblockMechanicStatusByUserIdCommandHandler> _logger;
    private readonly IMechanicCache _cache;

    public UnblockMechanicStatusByUserIdCommandHandler(
       IUnitOfWork unitOfWork,
       ILogger<UnblockMechanicStatusByUserIdCommandHandler> logger,
       IMechanicCache cache)
    {
        _cache = cache;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _asyncRetryPolicy = Policy
            .Handle<DbException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogError("Retry on {retryCount} failed. Waiting {timeSpan} before try again. Error: {message}", retryCount, timeSpan, exception.Message);
                });
    }

    public async Task<Result> Handle(UnblockMechanicStatusByUserIdCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
        try
        {
            var mechanic = await _unitOfWork.BaseUsers.GetMechanicUserByIdAsync(request.UserId);
            if (mechanic is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Mechanic not found", ResponseStatus.NotFound);
            }

            mechanic.UnblockAccount();

            _unitOfWork.BaseUsers.Update(mechanic);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);    

            return Result.Success(null, ResponseStatus.NoContent);
        }
        catch (DomainException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message, ResponseStatus.BadRequest);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError);
        }
    }
}

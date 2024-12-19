using Account.API.Applications.Services.Interfaces;
using Account.Domain.Events;
using Account.Domain.SeedWork;
using Core.CustomMessages;
using Core.Exceptions;
using Core.Results;
using Core.Utilities;
using MediatR;
using Polly;
using Polly.Retry;
using System.Data;

namespace Account.API.Applications.Commands.OrderTaskCommand.AcceptOrderByMechanicUserId;

public class AcceptOrderByMechanicUserIdCommandHandler : IRequestHandler<AcceptOrderByMechanicUserIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;
    private readonly ILogger<AcceptOrderByMechanicUserIdCommandHandler> _logger;
    private readonly IMechanicCache _mechanicCache;
    private readonly IMediator _mediator;

    public AcceptOrderByMechanicUserIdCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AcceptOrderByMechanicUserIdCommandHandler> logger,
        IMechanicCache mechanicCache,
        IMediator mediator)
    {
        _mediator = mediator;
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
                try
                {  
                    (var isSuccess, var buyerId) = await _mechanicCache.AcceptOrderByMechanic(
                        request.OrderId.ToString(), 
                        request.MechanicId.ToString());
                    
                    if (isSuccess)
                    { 
                        var @event = new AccountMechanicOrderAcceptedDomainEvent(
                            request.OrderId,
                            Guid.Parse(buyerId), 
                            request.MechanicId, 
                            mechanic.ToString(),
                            mechanic.PersonalInfo.ProfilePictureUrl ?? "",
                            mechanic.Rating);

                        await _mediator.Publish(@event);
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
                mechanic.AcceptJob(); 
                _unitOfWork.BaseUsers.Update(mechanic); 
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

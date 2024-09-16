using Account.Domain.Aggregates.OrderTaskAggregate;
using Account.Domain.SeedWork;
using Core.Results;
using Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.Common;

namespace Account.API.Applications.Commands.OrderTaskCommand.CreateOrderTask;

public class CreateOrderTaskCommandHandler: IRequestHandler<CreateOrderTaskCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderTaskCommandHandler> _logger;
    private readonly AsyncPolicy _asyncPolicy;

    public CreateOrderTaskCommandHandler(
       IUnitOfWork unitOfWork,
       ILogger<CreateOrderTaskCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _asyncPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>()
            .Or<DbException>()
            .WaitAndRetryAsync(5, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    LoggerHelper.LogError(logger, exception);
                });
    }

    public async Task<Result> Handle(CreateOrderTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _asyncPolicy.ExecuteAsync(async () =>
            {  
                var order = new OrderTaskWaitingMechanicAssign(request.OrderId, request.Latitude, request.Longitude);

                await _unitOfWork.OrderTasks.CreateOrderTaskWaitingMechanicAssignAsync(order); 

                await _unitOfWork.SaveChangesAsync();

                return Result.Success(null, ResponseStatus.NoContent);
            });

            return result;
        } 
        catch(Exception ex) 
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }
}

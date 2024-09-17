using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Core.Exceptions;
using Core.Results;
using Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly; 
using StackExchange.Redis;
using System.Data;
using System.Data.Common;

namespace Account.API.Applications.Commands.OrderTaskCommand.CreateOrderTask;

public class CreateOrderTaskCommandHandler: IRequestHandler<CreateOrderTaskCommand, Result>
{ 
    private readonly ILogger<CreateOrderTaskCommandHandler> _logger;
    private readonly AsyncPolicy _asyncPolicy;
    private readonly IMechanicCache _cache;
    public CreateOrderTaskCommandHandler( 
       ILogger<CreateOrderTaskCommandHandler> logger,
       IMechanicCache mechanicCache)
    {
        _cache = mechanicCache; 
        _logger = logger;
        _asyncPolicy = Policy
            .Handle<DBConcurrencyException>()
            .Or<DbUpdateException>()
            .Or<DbException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(1, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
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
                await _cache.CreateOrderToQueueAndHash(
                    new OrderTask(
                        request.BuyerId.ToString(), 
                        request.OrderId.ToString(), 
                        string.Empty, 
                        request.Latitude, 
                        request.Longitude, 
                        OrderTaskStatus.WaitingMechanic)); 
                   
                return Result.Success(null, ResponseStatus.Created); 
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

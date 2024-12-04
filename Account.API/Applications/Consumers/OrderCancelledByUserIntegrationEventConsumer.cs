using Account.API.Applications.Services.Interfaces;
using Core.Events.Ordering;
using MassTransit;
using Polly;
using Polly.Retry;

namespace Account.API.Applications.Consumers;

public class OrderCancelledByUserIntegrationEventConsumer(
    IMechanicCache mechanicCache) : IConsumer<OrderCancelledByBuyerIntegrationEvent>
{
    private readonly IMechanicCache _mechanicCache = mechanicCache;

    private readonly AsyncRetryPolicy policy = Policy
       .Handle<Exception>() 
       .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
           (exception, timeSpan, retryCount, context) =>
           { 
              
           }); 

    public async Task Consume(ConsumeContext<OrderCancelledByBuyerIntegrationEvent> context)
    {
        try
        { 
            await policy.ExecuteAsync(async () =>
            {
                (var isSuccess, var mechanicId) = await _mechanicCache.CancelOrderByUser(
                   context.Message.OrderId.ToString(),
                   context.Message.BuyerId.ToString());

                if (!isSuccess)
                {
                    throw new Exception($"Failed when cancelling the order {context.Message.OrderId} by buyer");
                }
            });
        }
        catch (Exception)
        { 

        } 
    }
}

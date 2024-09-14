
using Core.Utilities;
using Ordering.Domain.SeedWork;
using System.Data;

namespace Ordering.API.Applications.Services;

public class ScheduledOrderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledOrderWorker> _logger;
    public ScheduledOrderWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ScheduledOrderWorker> logger)
    {
        _scopeFactory = serviceScopeFactory;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        while (!stoppingToken.IsCancellationRequested) 
        {
            using var scope = _scopeFactory.CreateScope(); 
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();


            try
            {
                await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, stoppingToken);

                var orders = await unitOfWork.ScheduledOrders.GetOnTimeOrderWithUnpublishedEvent(30);

                if (orders is not null && orders.Any())
                {
                    foreach (var order in orders) 
                    {
                        // need to be tune up for performance   
                        var orderAggregate = await unitOfWork.Orders.GetByIdAsync(order.Id, stoppingToken);

                        if (orderAggregate is not null && orderAggregate.IsPaid)
                        {
                            orderAggregate.SetFindingMechanic();

                            unitOfWork.Orders.Update(orderAggregate);
                        }
                    }
                }

                await unitOfWork.CommitTransactionAsync(stoppingToken);
            }
            catch (Exception ex) 
            {
                LoggerHelper.LogError(_logger, ex);
                await unitOfWork.RollbackTransactionAsync(stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}

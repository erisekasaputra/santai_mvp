using Account.API.Applications.Services.Interfaces;
using Account.Domain.Aggregates.OrderTaskAggregate;
using Account.Domain.SeedWork;
using Core.Utilities;
using System.Data;

namespace Account.API.Applications.Services;

public class OrderWaitingMechanicConfirmExpiryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderWaitingMechanicConfirmExpiryJob> _logger;
    private IMechanicCache _mechanicCache;

    public OrderWaitingMechanicConfirmExpiryJob(
        IServiceScopeFactory serviceScopeFactory, 
        ILogger<OrderWaitingMechanicConfirmExpiryJob> logger)
    {
        _scopeFactory = serviceScopeFactory;
        _mechanicCache = null!;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        var delayMilliseconds = 1000;
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>();

            try
            {
                await _mechanicCache.Ping();
                 
                await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, stoppingToken);

                var orders = await unitOfWork.OrderTasks.GetOrdersExpiredMechanicConfirmationSkipLockedAsync(30);
                if (orders is not null && orders.Any())
                {
                    var orderProcessingTasks = new List<Task>();

                    foreach (var order in orders)
                    { 
                        orderProcessingTasks.Add(ProcessExpiredOrderAsync(order, unitOfWork, stoppingToken)); 
                    }
                     
                    await Task.WhenAll(orderProcessingTasks);

                    await _mechanicCache.Ping();
                    foreach(var order in orders)
                    {
                        await _mechanicCache.UnassignOrderFromMechanicAsync(order.MechanicId);
                    }

                    unitOfWork.OrderTasks.UpdateRangeOrdersConfirm(orders); 

                    await unitOfWork.CommitTransactionAsync(stoppingToken);
                }
                else
                { 
                    await unitOfWork.RollbackTransactionAsync(stoppingToken);
                }
                 
                delayMilliseconds = 2000;
            }
            catch (Exception ex)
            { 
                await unitOfWork.RollbackTransactionAsync(stoppingToken);
                LoggerHelper.LogError(_logger, ex);
                 
                delayMilliseconds = Math.Min(delayMilliseconds * 2, 5000); 
            }
             

            await Task.Delay(delayMilliseconds, stoppingToken);
        }
    }

    private async Task ProcessExpiredOrderAsync(
        OrderTaskWaitingMechanicConfirm orderWaitingConfirm, 
        IUnitOfWork unitOfWork, 
        CancellationToken stoppingToken)
    {
        try
        {
            // Retrieve the mechanic task
            var mechanicTask = await unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdSkipLockedAsync(orderWaitingConfirm.MechanicId);
            if (mechanicTask is null) return;

            // Retrieve the order waiting for mechanic confirmation
            var orderWaitingMechanicAssign = await unitOfWork.OrderTasks.GetOrderWaitingMechanicAssignByOrderIdWithSkipLockedAsync(orderWaitingConfirm.OrderId);
            if (orderWaitingMechanicAssign is null) return;

            // Reset the mechanic task and mark the order as expired
            mechanicTask.ResetOrder();
            orderWaitingConfirm.SetExpire();

            // Update mechanic task and order in unit of work
            unitOfWork.OrderTasks.UpdateMechanicTask(mechanicTask);
            orderWaitingMechanicAssign.DestroyMechanic(); 
            unitOfWork.OrderTasks.UpdateOrderAssign(orderWaitingMechanicAssign);
        }
        catch (Exception ex)
        { 
            _logger.LogError("Error processing expired order {OrderId} for mechanic {MechanicId}. \r\n Error: {error}", orderWaitingConfirm.OrderId, orderWaitingConfirm.MechanicId, ex.Message); 
            throw;
        }
    }
}





//public class OrderWaitingMechanicConfirmExpiryJob : BackgroundService
//{
//    private readonly IServiceScopeFactory _scopeFactory; 
//    private readonly IMechanicCache _mechanicCache;
//    private readonly ILogger<OrderWaitingMechanicConfirmExpiryJob> _logger;

//    public OrderWaitingMechanicConfirmExpiryJob( 
//        IServiceScopeFactory serviceScopeFactory, 
//        IMechanicCache mechanicCache,
//        ILogger<OrderWaitingMechanicConfirmExpiryJob> logger)
//    {
//        _scopeFactory = serviceScopeFactory; 
//        _mechanicCache = mechanicCache;
//        _logger = logger;
//    } 

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {

//        using var scope = _scopeFactory.CreateScope(); 
//        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//        while (!stoppingToken.IsCancellationRequested) 
//        {
//            try
//            {
//                await _mechanicCache.Ping();  
//                await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, stoppingToken); 

//                var orders = await unitOfWork.OrderTasks.GetOrdersExpiredMechanicConfirmationSkipLockedAsync(30); 
//                if (orders is not null)
//                {
//                    foreach (var order in orders)
//                    {

//                        var mechanicTask = await unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdSkipLockedAsync(order.MechanicId);
//                        if (mechanicTask is null)
//                        {
//                            continue;
//                        }

//                        var orderWaitingConfirm = await unitOfWork.OrderTasks.GetOrderByOrderIdWithSkipLockedAsync(order.OrderId);
//                        if (orderWaitingConfirm is null)
//                        {
//                            continue;
//                        }

//                        mechanicTask.ResetOrder();
//                        order.SetExpire();

//                        unitOfWork.OrderTasks.UpdateMechanicTask(mechanicTask);

//                        orderWaitingConfirm.DestroyMechanic();
//                        unitOfWork.OrderTasks.UpdateOrderConfirm(orderWaitingConfirm);
//                    }

//                    unitOfWork.OrderTasks.UpdateRangeOrdersConfirm(orders);
//                }

//                await unitOfWork.CommitTransactionAsync(stoppingToken);
//            }
//            catch (Exception ex)
//            {
//                await unitOfWork.RollbackTransactionAsync(stoppingToken);
//                LoggerHelper.LogError(_logger, ex);
//            }

//            await Task.Delay(2000);
//        } 
//    }
//}


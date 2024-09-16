using Account.API.Applications.Services.Interfaces;  
using Core.Utilities; 

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
        var delayMilliseconds = 5000;
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>();

            try
            {
                await _mechanicCache.Ping();

                delayMilliseconds = 2000;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex);
                delayMilliseconds = Math.Min(delayMilliseconds * 2, 5000);
            }


            await Task.Delay(delayMilliseconds, stoppingToken);
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


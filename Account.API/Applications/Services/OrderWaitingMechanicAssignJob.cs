using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Account.Domain.Aggregates.OrderTaskAggregate;
using Account.Domain.SeedWork;
using Core.Utilities; 
using System.Data;

namespace Account.API.Applications.Services; 
 
public class OrderWaitingMechanicAssignJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory; 
    private readonly ILogger<OrderWaitingMechanicAssignJob> _logger;
    private IMechanicCache _mechanicCache;

    public OrderWaitingMechanicAssignJob(
        IServiceScopeFactory serviceScopeFactory, 
        ILogger<OrderWaitingMechanicAssignJob> logger)
    {
        _mechanicCache = null!;
        _scopeFactory = serviceScopeFactory; 
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>();

            try
            {
                await _mechanicCache.Ping();
                await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, stoppingToken);

                var mechanicOrdersAssigned = new List<(MechanicAvailabilityCache, Guid)>();
                var orders = await unitOfWork.OrderTasks.GetOrdersUnassignedMechanicSkipLockedAsync(30);

                if (orders is not null && orders.Any())
                { 
                    foreach (var order in orders)
                    {
                        await ProcessOrder(order, mechanicOrdersAssigned, unitOfWork, stoppingToken);
                    } 

                    await _mechanicCache.Ping();

                    foreach ((var mechanic, var orderId) in mechanicOrdersAssigned)
                    {
                        await _mechanicCache.AssignOrderToMechanicAsync(mechanic, orderId);
                    }

                    unitOfWork.OrderTasks.UpdateRangeOrdersAssign(orders);
                    await unitOfWork.CommitTransactionAsync(stoppingToken);
                }
                else
                { 
                    await unitOfWork.RollbackTransactionAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(stoppingToken);
                LoggerHelper.LogError(_logger, ex);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ProcessOrder(
        OrderTaskWaitingMechanicAssign order,
        List<(MechanicAvailabilityCache, Guid)> mechanicOrdersAssigned,
        IUnitOfWork unitOfWork,
        CancellationToken stoppingToken)
    {
        try
        { 
            var mechanic = await _mechanicCache.FindAvailableMechanicAsync(
                order.OrderId,
                order.Latitude, 
                order.Longitude, 
                order.RetryAttemp * 5); 

            if (mechanic == null)
            {
                order.IncreaseRetryAttemp();
                return;
            }

            var mechanicFromSql = await unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdSkipLockedUnassignedOrderAsync(mechanic.MechanicId); 
            if (mechanicFromSql is null)
            {
                return;
            }

            var mechanicFromCache = await _mechanicCache.GetMechanicAsync(mechanic.MechanicId); 
            if (mechanicFromCache is null)
            {
                await _mechanicCache.Ping();
                await _mechanicCache.CreateMechanicHsetAsync(mechanic);
            }

            try
            {
                mechanicFromSql.AssignOrder(order.OrderId, order.Latitude, order.Longitude);
                order.AssignMechanic(mechanic.MechanicId);


                unitOfWork.OrderTasks.UpdateMechanicTask(mechanicFromSql);
                await unitOfWork.OrderTasks.CreateOrderTaskWaitingMechanicConfirmAsync(
                    new OrderTaskWaitingMechanicConfirm(
                        order.OrderId, 
                        mechanic.MechanicId, 
                        order.MechanicConfirmationExpire!.Value));


                mechanicOrdersAssigned.Add((mechanic, order.OrderId));
            }
            catch (DBConcurrencyException)
            {
                mechanicFromSql.ResetOrder();
                order.DestroyMechanic(); 

                _logger.LogWarning("Concurrency exception while assigning order {OrderId} to mechanic {MechanicId}", order.OrderId, mechanic.MechanicId);
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            await unitOfWork.RollbackTransactionAsync(stoppingToken);
            throw; 
        }
    }
}




//public class OrderWaitingMechanicAssignJob : BackgroundService
//{
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly IMechanicCache _mechanicCache;
//    private readonly ILogger<OrderWaitingMechanicAssignJob> _logger;

//    public OrderWaitingMechanicAssignJob(
//        IServiceScopeFactory serviceScopeFactory,
//        IMechanicCache mechanicCache,
//        ILogger<OrderWaitingMechanicAssignJob> logger)
//    {
//        _scopeFactory = serviceScopeFactory;
//        _mechanicCache = mechanicCache;
//        _logger = logger;
//    } 

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        using var scope = _scopeFactory.CreateScope(); 
//        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//        while(!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                await _mechanicCache.Ping(); 
//                await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, stoppingToken); 

//                var mechanicOrdersAssigned = new List<(MechanicAvailabilityCache, Guid)>();

//                var orders = await unitOfWork.OrderTasks.GetOrdersUnassignedMechanicSkipLockedAsync(30);
//                foreach (var order in orders)
//                {
//                    var mechanic = await _mechanicCache.FindAvailableMechanicAsync(
//                        order.Latitude,
//                        order.Longitude,
//                        order.RetryAttemp * 5);

//                    if (mechanic is null)
//                    {
//                        order.IncreaseRetryAttemp();
//                        continue;
//                    }

//                    var mechanicFromSql = await unitOfWork.OrderTasks.GetMechanicTaskByMechanicIdSkipLockedUnassignedOrderAsync(mechanic.MechanicId);

//                    if (mechanicFromSql is null)
//                    {
//                        continue;
//                    }

//                    var mechanicFromCache = await _mechanicCache.GetMechanicAsync(mechanic.MechanicId);

//                    try
//                    {
//                        mechanicFromSql.AssignOrder(order.OrderId, order.Latitude, order.Longitude);

//                        order.AssignMechanic(mechanic.MechanicId);

//                        unitOfWork.OrderTasks.UpdateMechanicTask(mechanicFromSql);

//                        await unitOfWork.OrderTasks
//                            .CreateOrderTaskWaitingMechanicConfirmAsync(new OrderTaskWaitingMechanicConfirm(order.OrderId, mechanic.MechanicId, order.MechanicConfirmationExpire!.Value));

//                        mechanicOrdersAssigned.Add((mechanic, order.OrderId));
//                    }
//                    catch (DBConcurrencyException)
//                    {
//                        continue;
//                    }
//                    catch (Exception ex)
//                    {
//                        LoggerHelper.LogError(_logger, ex);
//                        await unitOfWork.RollbackTransactionAsync();
//                        return;
//                    }
//                }

//                await _mechanicCache.Ping();

//                foreach ((var mechanic, var orderId) in mechanicOrdersAssigned)
//                {
//                    await _mechanicCache.AssignOrderToMechanicAsync(mechanic, orderId);
//                }

//                unitOfWork.OrderTasks.UpdateRangeOrdersAssign(orders);
//                await unitOfWork.CommitTransactionAsync(stoppingToken);
//            }
//            catch (Exception ex)
//            {
//                await unitOfWork.RollbackTransactionAsync(stoppingToken);
//                LoggerHelper.LogError(_logger, ex);
//            }
//        } 
//    }
//}
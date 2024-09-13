using Account.API.Applications.Services.Interfaces;
using Account.Domain.Aggregates.OrderTaskAggregate;
using Account.Domain.SeedWork;
using Core.Utilities;
using MassTransit;
using System.Data;

namespace Account.API.Applications.Services;

public class OrderJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMechanicCache _mechanicCache;
    private readonly ILogger<OrderJobService> _logger;
    public OrderJobService(
        IUnitOfWork unitOfWork,
        IMechanicCache mechanicCache,
        ILogger<OrderJobService> logger)
    {
        _unitOfWork = unitOfWork;
        _mechanicCache = mechanicCache;
        _logger = logger;   
    }

    public void OrderWaitingMechanicAssignSync()
    {
        OrderWaitingMechanicAssign().GetAwaiter().GetResult();
    }

    public void OrderWaitingMechanicConfirmSync()
    {
        OrderWaitingMechanicConfirm().GetAwaiter().GetResult();
    }

    public async Task OrderWaitingMechanicConfirm()
    {
        await Task.Delay(1000);
    }

    public async Task OrderWaitingMechanicAssign()
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable); 
            var orders = await _unitOfWork.OrderTasks.GetOrderWithUnassignedMechanic(100);
               
            foreach (var order in orders) 
            {
                var mechanic = await _mechanicCache.FindAvailableMechanicAsync(order.Latitude, order.Longitude, order.RetryAttemp * 5); 
                if (mechanic is null)
                {
                    order.IncreaseRetryAttemp(); 
                    continue;
                }

                var mechanicFromSql = await _unitOfWork.OrderTasks.GetMechanicTaskByIdWithSkipLockAsync(mechanic.MechanicId);
                if (mechanicFromSql is null)
                {
                    continue;
                }

                try
                {
                    mechanicFromSql.AssignOrder(order.Id, order.Latitude, order.Longitude);
                    
                    await _mechanicCache.AssignOrderToMechanicAsync(mechanic, order.Id); 
                    
                    order.AssignMechanic(mechanic.MechanicId);

                    _unitOfWork.OrderTasks.UpdateMechanicTask(mechanicFromSql);

                    var mechanicConfirm = new OrderTaskWaitingMechanicConfirm(order.Id, mechanic.MechanicId);

                    await _unitOfWork.OrderTasks.CreateOrderTaskWaitingMechanicConfirmAsync(mechanicConfirm);
                }
                catch (ConcurrencyException)
                {
                    continue;
                } 
                catch (Exception ex)  
                {
                    LoggerHelper.LogError(_logger, ex);
                    await _unitOfWork.RollbackTransactionAsync();
                    return;
                }
            } 

            _unitOfWork.OrderTasks.UpdateRangeOrders(orders); 
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex) 
        {
            await _unitOfWork.RollbackTransactionAsync();
            LoggerHelper.LogError(_logger, ex);   
        }
    }
}

using Account.Domain.Aggregates.OrderTaskAggregate;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore; 

namespace Account.Infrastructure.Repositories;

public class OrderTaskRepository : IOrderTaskRepository
{
    private readonly AccountDbContext _dbContext;

    public OrderTaskRepository(AccountDbContext accountDbContext)
    {
        _dbContext = accountDbContext;  
    } 
    public async Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdSkipLockedAsync(Guid mechanicId)
    { 
        var mechanic = await _dbContext.MechanicOrderTasks 
           .FromSqlRaw("SELECT * FROM [MechanicOrderTasks] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [MechanicId] = @id ", new SqlParameter("@id", mechanicId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdAsync(Guid mechanicId)
    { 
        var mechanic = await _dbContext.MechanicOrderTasks
           .FromSqlRaw("SELECT * FROM [MechanicOrderTasks] WHERE [MechanicId] = @id ", new SqlParameter("@id", mechanicId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdSkipLockedUnassignedOrderAsync(Guid mechanicId)
    {
        var mechanic = await _dbContext.MechanicOrderTasks
           .FromSqlRaw("SELECT * FROM [MechanicOrderTasks] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [MechanicId] = @id AND [IsOrderAssigned] = 0 ", new SqlParameter("@id", mechanicId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdUnassignedOrderAsync(Guid mechanicId)
    {
        var mechanic = await _dbContext.MechanicOrderTasks
           .FromSqlRaw("SELECT * FROM [MechanicOrderTasks] WHERE [MechanicId] = @id AND [IsOrderAssigned] = 0 ", new SqlParameter("@id", mechanicId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<IEnumerable<OrderTaskWaitingMechanicAssign>> GetOrdersUnassignedMechanicSkipLockedAsync(int rowNumber)
    {
        if (rowNumber < 1)
        {
            rowNumber = 1;
        }

        var query = $"SELECT TOP {rowNumber} * FROM [{nameof(_dbContext.OrderTaskWaitingMechanicAssigns)}] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [{nameof(OrderTaskWaitingMechanicAssign.IsOrderCompleted)}] = 0 AND [{nameof(OrderTaskWaitingMechanicAssign.IsMechanicAssigned)}] = 0 ORDER BY [{nameof(OrderTaskWaitingMechanicAssign.CreatedAt)}] ASC ";

        var orders = await _dbContext
            .OrderTaskWaitingMechanicAssigns
            .FromSqlRaw(query)
            .ToListAsync(); 
        return orders;
    }  

    public async Task<IEnumerable<OrderTaskWaitingMechanicConfirm>> GetOrdersExpiredMechanicConfirmationSkipLockedAsync(int rowNumber)
    {
        if (rowNumber < 1)
        {
            rowNumber = 1;
        } 

        var query = $"SELECT TOP {rowNumber} * FROM [{nameof(_dbContext.OrderTaskWaitingMechanicConfirms)}] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [{nameof(OrderTaskWaitingMechanicConfirm.IsProcessed)}] = 0 AND [{nameof(OrderTaskWaitingMechanicConfirm.IsExpiryProcessed)}] = 0 AND GETUTCDATE() >= [ExpiredAt] ORDER BY [{nameof(OrderTaskWaitingMechanicConfirm.CreatedAt)}] ASC ";

        var orders = await _dbContext
            .OrderTaskWaitingMechanicConfirms
            .FromSqlRaw(query)
            .ToListAsync();

        return orders;
    }

    public async Task CreateOrderTaskWaitingMechanicAssignAsync(OrderTaskWaitingMechanicAssign assignTask)
    {
        await _dbContext.OrderTaskWaitingMechanicAssigns.AddAsync(assignTask);
    } 

    public async Task CreateOrderTaskWaitingMechanicConfirmAsync(OrderTaskWaitingMechanicConfirm confirmTask)
    { 
        await _dbContext.OrderTaskWaitingMechanicConfirms.AddAsync(confirmTask); 
    }

    public void UpdateMechanicTask(MechanicOrderTask mechanicTask)
    { 
        _dbContext.MechanicOrderTasks.Update(mechanicTask);
    }

    public void UpdateRangeOrdersAssign(IEnumerable<OrderTaskWaitingMechanicAssign> orders)
    {
        _dbContext.OrderTaskWaitingMechanicAssigns.UpdateRange(orders);
    }

    public void UpdateRangeOrdersConfirm(IEnumerable<OrderTaskWaitingMechanicConfirm> orders)
    { 
        _dbContext.OrderTaskWaitingMechanicConfirms.UpdateRange(orders);
    }

    public async Task<OrderTaskWaitingMechanicAssign?> GetOrderWaitingMechanicAssignByOrderIdWithSkipLockedAsync(Guid orderId)
    { 
        var mechanic = await _dbContext.OrderTaskWaitingMechanicAssigns
           .FromSqlRaw("SELECT * FROM [OrderTaskWaitingMechanicAssigns] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [OrderId] = @id ", new SqlParameter("@id", orderId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<OrderTaskWaitingMechanicAssign?> GetOrderWaitingMechanicAssignByOrderIdAsync(Guid orderId)
    {
        var mechanic = await _dbContext.OrderTaskWaitingMechanicAssigns
           .FromSqlRaw("SELECT * FROM [OrderTaskWaitingMechanicAssigns] WHERE [OrderId] = @id ", new SqlParameter("@id", orderId))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public void UpdateOrderAssign(OrderTaskWaitingMechanicAssign order)
    {
        _dbContext.OrderTaskWaitingMechanicAssigns.Update(order);
    }

    public void UpdateOrderConfirm(OrderTaskWaitingMechanicConfirm order)
    {
        _dbContext.OrderTaskWaitingMechanicConfirms.Update(order);
    }

    public async Task<OrderTaskWaitingMechanicConfirm?> GetOrderWaitingMechanicConfirmByOrderIdWithSkipLockedAsync(Guid orderId)
    {
        var mechanic = await _dbContext.OrderTaskWaitingMechanicConfirms
          .FromSqlRaw("SELECT * FROM [OrderTaskWaitingMechanicConfirms] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [OrderId] = @id ", new SqlParameter("@id", orderId))
          .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<OrderTaskWaitingMechanicConfirm?> GetOrderWaitingMechanicConfirmByOrderIdAsync(Guid orderId)
    {
        var mechanic = await _dbContext.OrderTaskWaitingMechanicConfirms
          .FromSqlRaw("SELECT * FROM [OrderTaskWaitingMechanicConfirms] WHERE [OrderId] = @id ", new SqlParameter("@id", orderId))
          .FirstOrDefaultAsync();

        return mechanic;
    }

    public void RemoveMechanicTask(MechanicOrderTask task)
    { 
        _dbContext.MechanicOrderTasks.Remove(task);
    }
}

using Account.Domain.Aggregates.OrderTaskAggregate;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Account.Infrastructure.Repositories;

public class OrderTaskRepository : IOrderTaskRepository
{
    private readonly AccountDbContext _dbContext;

    public OrderTaskRepository(AccountDbContext accountDbContext)
    {
        _dbContext = accountDbContext;  
    }

    public async Task CreateOrderTaskWaitingMechanicAssignAsync(OrderTaskWaitingMechanicAssign assignTask)
    {
        await _dbContext.OrderTaskWaitingMechanicAssigns.AddAsync(assignTask);
    }

    public async Task<MechanicOrderTask?> GetMechanicTaskByIdWithSkipLockAsync(Guid id)
    {
        var mechanic = await _dbContext.MechanicOrderTasks 
           .FromSqlRaw(@"SELECT *
               FROM [MechanicOrderTasks] 
               WITH (UPDLOCK, ROWLOCK, READPAST) 
               WHERE [Id] = @id AND IsOrderAssigned = false;", new SqlParameter("@id", id))
           .FirstOrDefaultAsync();

        return mechanic;
    }

    public async Task<IEnumerable<OrderTaskWaitingMechanicAssign>> GetOrderWithUnassignedMechanic(int rowNumber)
    {
        if (rowNumber < 1)
        {
            rowNumber = 1;
        }

        var query = $@"SELECT TOP {rowNumber} *
                FROM [OrderTaskWaitingMechanicAssigns] 
                WITH (UPDLOCK, ROWLOCK, READPAST) 
                WHERE [IsOrderCompleted] = false AND [IsMechanicAssigned] = false
                            ORDER BY [CreatedAt] ASC;";

        var orders = await _dbContext
            .OrderTaskWaitingMechanicAssigns
            .FromSqlRaw(query)
            .ToListAsync(); 
        return orders;
    }

    public async Task CreateOrderTaskWaitingMechanicConfirmAsync(OrderTaskWaitingMechanicConfirm confirmTask)
    { 
        await _dbContext.OrderTaskWaitingMechanicConfirms.AddAsync(confirmTask); 
    }

    public void UpdateMechanicTask(MechanicOrderTask mechanicTask)
    { 
        _dbContext.MechanicOrderTasks.Update(mechanicTask);
    }

    public void UpdateRangeOrders(IEnumerable<OrderTaskWaitingMechanicAssign> orders)
    {
        _dbContext.OrderTaskWaitingMechanicAssigns.UpdateRange(orders);
    }
}

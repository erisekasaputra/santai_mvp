using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates.ScheduledOrderAggregate;

namespace Ordering.Infrastructure.Repositories;

public class ScheduledOrderRepository : IScheduledOrderRepository
{
    private readonly OrderDbContext _context;
    public ScheduledOrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ScheduledOrder>> GetOnTimeOrderWithUnpublishedEvent(int rowNumber)
    {
        if (rowNumber < 1)
        {
            rowNumber = 1;
        }


        var query = $"SELECT TOP {rowNumber} * FROM [{nameof(_context.ScheduledOrders)}] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [{nameof(ScheduledOrder.IsEventProcessed)}] = 0 AND GETUTCDATE() >= [{nameof(ScheduledOrder.ScheduledAt)}] ORDER BY [{nameof(ScheduledOrder.ScheduledAt)}] ASC ";

        var orders = await _context
            .ScheduledOrders
            .FromSqlRaw(query)
            .ToListAsync();
        return orders;
    }

    public void UpdateOnTimeOrderWithPublishedEvent(IEnumerable<ScheduledOrder> orders)
    {
        _context.ScheduledOrders.UpdateRange(orders);
    }
    

    public async Task CraeteAsync(ScheduledOrder order)
    {
        await _context.ScheduledOrders.AddAsync(order);
    }
}

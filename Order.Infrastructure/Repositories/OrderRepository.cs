using Microsoft.EntityFrameworkCore;
using Order.Domain.Aggregates.OrderAggregate;

namespace Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;
    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    public async Task<Ordering> CreateAsync(Ordering order, CancellationToken cancellationToken)
    { 
        var entity = await _dbContext.Orderings.AddAsync(order, cancellationToken);
        return entity.Entity;
    }

    public async Task<Ordering?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orderings
            .Include(order => order.Fleets)
            .Include(order => order.LineItems)
            .Include(order => order.Payment)
            .Include(order => order.Cancellation)
            .Include(order => order.Mechanic)
            .Include(order => order.Buyer)
            .Include(order => order.Coupon)
            .Include(order => order.Fees) 
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(Ordering order)
    {
        _dbContext.Orderings.Update(order);
    }
}

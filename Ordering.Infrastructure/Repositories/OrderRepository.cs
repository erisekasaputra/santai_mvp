using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Aggregates.OrderAggregate; 

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;
    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Orders.AddAsync(order, cancellationToken);
        return entity.Entity;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
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

    public void Update(Order order)
    {
        _dbContext.Orders.Update(order);
    }
}

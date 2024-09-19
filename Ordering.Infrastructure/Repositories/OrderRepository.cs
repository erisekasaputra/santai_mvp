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

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Orders.AddAsync(order, cancellationToken);
        return entity.Entity;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders 
            .Include(order => order.LineItems)
            .Include(order => order.Payment)
            .Include(order => order.Cancellation)
            .Include(order => order.Mechanic)
            .Include(order => order.Buyer)
            .Include(order => order.Discount)
            .Include(order => order.Fees)
            .Include(order => order.Fleets)
            .ThenInclude(fleet => fleet.BasicInspections)
            .Include(order => order.Fleets)
            .ThenInclude(fleet => fleet.PreServiceInspections)
            .ThenInclude(preService => preService.PreServiceInspectionResults)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(Order order)
    {
        _dbContext.Orders.Update(order);
    }

    public async Task<Order?> GetByIdAndUserIdNoTrackingAsync(Guid orderId, Guid buyerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.LineItems)
            .Include(order => order.Payment)
            .Include(order => order.Cancellation)
            .Include(order => order.Mechanic)
            .Include(order => order.Buyer)
            .Include(order => order.Discount)
            .Include(order => order.Fees)
            .Include(order => order.Fleets)
            .ThenInclude(fleet => fleet.BasicInspections)
            .Include(order => order.Fleets)
            .ThenInclude(fleet => fleet.PreServiceInspections)
            .ThenInclude(preService => preService.PreServiceInspectionResults)
            .Where(x => x.Id == orderId && x.Buyer.BuyerId == buyerId)
            .FirstOrDefaultAsync(cancellationToken);
    }



    public async Task<(int TotalCount, int TotalPages, IEnumerable<Order> Orders)> GetPaginatedOrders(
        Guid? userId, int pageNumber, int pageSize)
    {
        var query = _dbContext.Orders.AsQueryable();
         
        if (userId is not null && userId.HasValue && userId != Guid.Empty)
        {
            query = query.Where(x => x.Buyer.BuyerId == userId);
        }

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .AsNoTracking() 
            .Include(order => order.LineItems)
            .Include(order => order.Payment)
            .Include(order => order.Cancellation)
            .Include(order => order.Mechanic)
            .Include(order => order.Buyer)
            .Include(order => order.Discount)
            .Include(order => order.Fees)
            .Include(order => order.Fleets) 
            .OrderBy(x => x.Status)
            .ThenBy(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public async Task<string?> GetOrderSecretByOrderIdAndUserId(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Where(x => x.Id == orderId && x.Buyer.BuyerId == userId)
            .Select(x => x.Secret) 
            .FirstOrDefaultAsync(cancellationToken);
    }
}

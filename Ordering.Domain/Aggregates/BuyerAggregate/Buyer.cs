using Core.Enumerations;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.BuyerAggregate;

public class Buyer : Entity
{
    public Guid OrderingId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Order Ordering { get; private set; }
    public string Name { get; private set; }
    public UserType BuyerType { get; private set; }
    public Buyer(Guid orderingId, Guid buyerId, string name, UserType buyerType)
    {
        OrderingId = orderingId;
        BuyerId = buyerId;
        Name = name;
        BuyerType = buyerType;
        Ordering = null!;
    }
}

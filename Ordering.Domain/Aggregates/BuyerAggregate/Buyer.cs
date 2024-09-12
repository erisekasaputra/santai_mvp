using Core.Enumerations;
using Ordering.Domain.Aggregates.OrderAggregate;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.Aggregates.BuyerAggregate;

public class Buyer : Entity
{
    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Order Order { get; private set; }
    public string Name { get; private set; }
    public UserType BuyerType { get; private set; }
    public Buyer(
        Guid orderId,
        Guid buyerId,
        string name,
        UserType buyerType)
    {
        OrderId = orderId;
        BuyerId = buyerId;
        Name = name;
        BuyerType = buyerType;
        Order = null!;
    }
}

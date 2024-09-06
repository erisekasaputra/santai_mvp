using Order.Domain.Enumerations;
using Order.Domain.SeedWork;

namespace Order.Domain.Aggregates.BuyerAggregate;

public class Buyer : Entity
{
    public string Name { get; private set; } 
    public UserType BuyerType { get; private set; }
    public Buyer(Guid id, string name, UserType buyerType)
    {
        Id = id;
        Name = name;
        BuyerType = buyerType;
    }
}

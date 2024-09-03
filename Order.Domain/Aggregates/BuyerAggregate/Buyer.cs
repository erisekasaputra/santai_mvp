using Order.Domain.Enumerations;
using Order.Domain.SeedWork;

namespace Order.Domain.Aggregates.BuyerAggregate;

public class Buyer : Entity
{
    public string Name { get; private set; } 
    public UserType UserType { get; private set; }
    public Buyer(Guid id, string name, UserType userType)
    {
        Id = id;
        Name = name;
        UserType = userType;
    }
}

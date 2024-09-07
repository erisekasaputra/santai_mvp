using Order.Domain.Aggregates.OrderAggregate;
using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.MechanicAggregate;

public class Mechanic : Entity
{ 
    public Guid OrderingId { get; private set; }
    public Ordering? Ordering { get; private set; }
    public Guid MechanicId { get; private set; }
    public string Name { get; private set; }
    public Rating? Rating { get; private set; } 
    public decimal Performance { get; private set; }

    public bool IsRated => Rating is not null && Rating.Value > 0.0M;
    public Mechanic(Guid orderingId, Guid mechanicId, string name, decimal perfomance)
    {
        OrderingId = orderingId;
        Id = mechanicId;
        Name = name;  
        Performance = perfomance;

    }

    public void SetMechanicRating(decimal rating, string? comment)
    {
        if (IsRated) 
        {
            throw new DomainException("Can not set rating twice");
        }

        Rating = new Rating(rating, comment); 
    } 
}

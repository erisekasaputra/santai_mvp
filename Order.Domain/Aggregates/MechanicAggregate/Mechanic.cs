using Order.Domain.Exceptions;
using Order.Domain.SeedWork;
using Order.Domain.ValueObjects;

namespace Order.Domain.Aggregates.MechanicAggregate;

public class Mechanic : Entity
{
    public string Name { get; private set; }
    public Rating? Rating { get; private set; }
    public bool IsRated { get; private set; }
    public decimal Performance { get; private set; }
    public Mechanic(Guid id, string name, decimal perfomance)
    {
        Id = id;
        Name = name; 
        IsRated = false;
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

using MediatR;

namespace Account.Domain.SeedWork;

public abstract class Entity
{
    private List<INotification> _domainEvents;

    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public Guid Id { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        _domainEvents = [];
    }

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        Entity item = (Entity)obj;

        return item.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null);
        }
        else
        {
            return left.Equals(right);
        }
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}

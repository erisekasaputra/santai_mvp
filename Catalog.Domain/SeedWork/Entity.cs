using MediatR;

namespace Catalog.Domain.SeedWork;


public abstract class Entity<T> 
{
    private int? _requestedHashCode;
    private T? _id;
    public virtual T Id
    {
        get
        {
            return _id ?? throw new ArgumentNullException(nameof(Id));
        }
        protected set
        {
            _id = value;
        }
    }

    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification> DomainEvents => (_domainEvents ?? []).AsReadOnly();

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

    public bool IsTransient()
    {
        return Equals(Id, default);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity<T>)
            return false;

        if (Object.ReferenceEquals(this, obj))
            return true;

        if (this.GetType() != obj.GetType())
            return false;

        Entity<T> item = (Entity<T>)obj;

        if (item.IsTransient() || this.IsTransient()) 
        {
            return false;
        }
        else
        {
            return Equals(item.Id, Id);
        }
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                _requestedHashCode = Id?.GetHashCode() ^ 31;
            } 

            return _requestedHashCode is not null ? _requestedHashCode.Value : throw new ArgumentNullException(nameof(Id));
        }
        else
            return base.GetHashCode();

    }
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        if (left is null)
        { 
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}

using Account.Domain.Events;
using Account.Domain.SeedWork;
using Core.Exceptions;

namespace Account.Domain.Aggregates.OrderTaskAggregate;

public class OrderTaskWaitingMechanicAssign : Entity
{
    public Guid OrderId { get; private set; }
    public Guid? MechanicId { get; private set; }
    public DateTime? MechanicConfirmationExpire { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int RetryAttemp { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsMechanicAssigned { get; private set; }
    public bool IsOrderCompleted { get; private set; }

    public OrderTaskWaitingMechanicAssign()
    {
        
    }

    public OrderTaskWaitingMechanicAssign(
        Guid orderid, double latitude, double longitude)
    {
        OrderId = orderid;
        RetryAttemp = 1;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = DateTime.UtcNow;
        MechanicConfirmationExpire = null;
        IsMechanicAssigned = false;
        IsOrderCompleted = false;
    }

    public void CompleteOrder()
    {
        if (!IsMechanicAssigned)
        {
            throw new DomainException("Mechanic has bot been assigned");
        }

        IsOrderCompleted = true;
        RaiseOrderCompletedDomainEvent();
    }

    public void IncreaseRetryAttemp()
    {
        if (RetryAttemp >= 5)
        {
            return;
        }

        RetryAttemp++;
    }

    public void CancelByMechanic()
    {
        RetryAttemp = 1;
        MechanicId = null;
        IsMechanicAssigned = false;
    }

    public void RejectByMechanic()
    {
        RetryAttemp = 1;
        MechanicId = null;
        IsMechanicAssigned = false;
    }

    public void AssignMechanic(Guid mechanicId)
    {
        if (IsOrderCompleted)
        {
            throw new InvalidOperationException("Order has completed");
        }

        if (mechanicId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(mechanicId));
        }

        if (MechanicId is not null && MechanicId != Guid.Empty)
        {
            throw new InvalidOperationException($"Mechanic has been assigned to an order {OrderId}");
        }

        MechanicConfirmationExpire = DateTime.UtcNow.AddSeconds(60);
        IsMechanicAssigned = true;
        MechanicId = mechanicId;  
        RaiseMechanicAssignedDomainEvent();
    }

    private void RaiseOrderCompletedDomainEvent()
    {
        AddDomainEvent(new OrderCompletedDomainEvent(OrderId));
    }

    private void RaiseMechanicAssignedDomainEvent()
    {
        AddDomainEvent(new MechanicAssignedDomainEvent(OrderId, MechanicId!.Value));
    }
}

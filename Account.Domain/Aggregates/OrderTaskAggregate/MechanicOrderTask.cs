using Account.Domain.Events;
using Account.Domain.SeedWork;
using Core.Exceptions;
using System.Data;

namespace Account.Domain.Aggregates.OrderTaskAggregate;

public class MechanicOrderTask : Entity
{
    public Guid MechanicId { get; private set; }
    public Guid? OrderId { get; private set; }  
    public bool IsOrderAssigned { get; private set; }  
    public bool IsActive { get; private set; }
    public MechanicOrderTask()
    {

    }

    public MechanicOrderTask(
        Guid mechanicId,
        Guid? orderId)
    {
        MechanicId = mechanicId;
        OrderId = orderId; 
        IsOrderAssigned = false; 
    }

    public void Activate()
    { 
        if (IsActive)
        {
            throw new DomainException("Mechanic account has been activated");
        }

        IsActive = true;
        RaiseMechanicActivatedDomainEvent();
    }

    private void RaiseMechanicActivatedDomainEvent()
    {
        AddDomainEvent(new MechanicActivatedDomainEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DomainException("Mechanic account has been deactivated");
        } 

        IsActive = false;
        RaiseMechanicDeactivatedDomainEvent();
    }

    private void RaiseMechanicDeactivatedDomainEvent()
    {
        AddDomainEvent(new MechanicDeactivatedDomainEvent(this));
    }

    public bool ResetOrder()
    {  
        IsOrderAssigned = false; 
        OrderId = null; 
        return true;
    }

    public void AssignOrder(
        Guid orderId,
        double latitude,
        double longitude)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(orderId));
        }

        if (IsOrderAssigned)
        {
            throw new InvalidOperationException("Order has been assigned to this mechanic");
        }

        OrderId = orderId; 
        IsOrderAssigned = true; 
    } 
}

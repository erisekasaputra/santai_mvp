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
    public bool IsAcceptedByMechanic { get; private set; }

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
        IsAcceptedByMechanic = false;
    }

    public void AcceptOrderByMechanic(Guid mechanicId)
    {
        if (MechanicId is null || !IsMechanicAssigned)
        {
            throw new DomainException("Mechanic has not been assigned");
        }

        if (mechanicId != MechanicId)
        {
            throw new DomainException("Can not accepting order");
        }


        IsAcceptedByMechanic = true;

        RaiseAccountMechanicOrderAcceptedDomainEvent();
    }

    private void RaiseAccountMechanicOrderAcceptedDomainEvent()
    {
        AddDomainEvent(new AccountMechanicOrderAcceptedDomainEvent(OrderId, MechanicId!.Value));
    }

    public void CompleteOrder(Guid mechanicId)
    {
        if (!IsMechanicAssigned)
        {
            throw new DomainException("Mechanic has bot been assigned");
        }

        if (mechanicId == MechanicId) 
        { 
            throw new DomainException("Can not accepting order");
        }

        IsOrderCompleted = true; 
    }

    public void IncreaseRetryAttemp()
    {
        if (RetryAttemp >= 5)
        {
            return;
        }

        RetryAttemp++;
    }

    public void DestroyMechanic(Guid mechanicId)
    {  
        if (MechanicId != mechanicId)
        {
            throw new DomainException("Mechanic id is missmatch");
        }

        RetryAttemp = 1;
        MechanicId = null;
        IsMechanicAssigned = false;
        IsAcceptedByMechanic = false;
    }


    public void DestroyMechanic()
    {  
        RetryAttemp = 1;
        MechanicId = null;
        IsMechanicAssigned = false;
        IsAcceptedByMechanic = false;
    }

    public void AssignMechanic(Guid mechanicId)
    {
        if (IsOrderCompleted)
        {
            throw new DomainException("Order has completed");
        }

        if (mechanicId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(mechanicId));
        } 

        MechanicConfirmationExpire = DateTime.UtcNow.AddSeconds(62);
        IsMechanicAssigned = true;
        MechanicId = mechanicId;
        IsAcceptedByMechanic = false;
    } 
}

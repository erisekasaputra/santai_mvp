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

    public void AcceptOrderByMechanic()
    {
        IsAcceptedByMechanic = true;
    }

    public void CompleteOrder()
    {
        if (!IsMechanicAssigned)
        {
            throw new DomainException("Mechanic has bot been assigned");
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

using Account.Domain.SeedWork;
using Core.Exceptions;

namespace Account.Domain.Aggregates.OrderTaskAggregate;

public class MechanicOrderTask : Entity
{
    public Guid MechanicId { get; private set; }
    public Guid? OrderId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsOrderAssigned { get; private set; }  
    public MechanicOrderTask()
    {

    }

    public MechanicOrderTask(
        Guid mechanicId,
        Guid? orderId,
        double latitude,
        double longitude)
    {
        MechanicId = mechanicId;
        OrderId = orderId;
        Latitude = latitude;
        Longitude = longitude;
        IsOrderAssigned = false; 
    } 

    public void ResetOrder()
    {
        IsOrderAssigned = false; 
        OrderId = null;
        Latitude = 0;
        Longitude = 0; 
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
            throw new DomainException("Order has been assigned to this mechanic");
        }

        OrderId = orderId;
        Latitude = latitude;
        Longitude = longitude;
        IsOrderAssigned = true; 
    } 
}

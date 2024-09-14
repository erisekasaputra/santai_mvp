using Account.Domain.SeedWork; 
using Microsoft.EntityFrameworkCore;

namespace Account.Domain.Aggregates.OrderTaskAggregate;

public class OrderTaskWaitingMechanicConfirm : Entity
{
    public Guid OrderId { get; private set; }
    public Guid MechanicId { get; private set; }
    public bool IsExpiryProcessed { get; private set; }
    public DateTime ExpiredAt { get; private set; } 
    public bool IsProcessed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public OrderTaskWaitingMechanicConfirm()
    {
        
    }
    public OrderTaskWaitingMechanicConfirm(
        Guid orderId,
        Guid mechanicId,
        DateTime mechanicConfirmExpiry)
    {
        OrderId = orderId;
        MechanicId = mechanicId;
        ExpiredAt = mechanicConfirmExpiry;
        IsProcessed = false;
        CreatedAt = DateTime.UtcNow;
    }   
     
    public void SetDelete()
    {
        SetExpire();
    }

    public void SetExpire()
    {
        IsExpiryProcessed = true;
        SetEntityState(EntityState.Deleted); 
    }

    public bool Proceed()
    {
        if (IsProcessed) return true;

        if (DateTime.UtcNow >= ExpiredAt)
        {
            IsProcessed = false;
            IsExpiryProcessed = true;
            return false;
        }

        IsProcessed = true;
        return true;
    }
}

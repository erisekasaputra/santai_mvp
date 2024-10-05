using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations; 
using Account.Domain.SeedWork;
using Core.Exceptions;

namespace Account.Domain.Aggregates.LoyaltyAggregate;

public class LoyaltyProgram : Entity, IAggregateRoot
{   
    public Guid LoyaltyUserId { get; private init; } 
    public BaseUser BaseUser { get; private set; }  
    public int LoyaltyPoints { get; private set; } 
    public LoyaltyTier LoyaltyTier { get; private set; } 
      
    public LoyaltyProgram(
        Guid loyaltyUserId,
        int loyaltyPoints)
    {  
        LoyaltyUserId = loyaltyUserId;
        LoyaltyPoints = loyaltyPoints;
        LoyaltyTier = LoyaltyTier.Basic;
        BaseUser = null!;
    } 

    public void SetPoint(int point)
    {
        LoyaltyPoints = point;
    }

    public void AddPoint(int point)
    {
        LoyaltyPoints += point;

        RaisePointAddedDomainEvent();
    }

    public void ReducePoint(int point)
    {
        if ((LoyaltyPoints - point) < 0)
        {
            throw new DomainException("Point is not enough");
        }

        LoyaltyPoints -= point;

        RaisePointReducedDomainEvent();
    }

    public void UpdateLoyaltyTier(LoyaltyTier tier)
    {
        if (LoyaltyTier == tier)
        {
            return;
        } 

        var previousTier = LoyaltyTier; 

        LoyaltyTier = tier; 

        if (LoyaltyTier > previousTier)
        {
            RaiseTierUpgradedDomainEvent();
            return;
        }

        RaiseTierDowngradedDomainEvent(); 
    }

    private void RaisePointReducedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaisePointAddedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseTierDowngradedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseTierUpgradedDomainEvent()
    {
        throw new NotImplementedException();
    }
}
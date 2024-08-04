using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.LoyaltyAggregate;

public class LoyaltyProgram : Entity, IAggregateRoot
{   
    public Guid LoyaltyUserId { get; private set; }

    public User User { get; private set; } 

    public int LoyaltyPoints { get; private set; }

    public LoyaltyTier LoyaltyTier { get; private set; } 

    public LoyaltyProgram()
    {

    }
     
    public LoyaltyProgram(
        Guid loyaltyUserId,
        int loyaltyPoints,
        LoyaltyTier loyaltyTier)
    {  
        LoyaltyUserId = loyaltyUserId;
        LoyaltyPoints = loyaltyPoints;
        LoyaltyTier = LoyaltyTier.Basic; 
    } 

    public void SetPoint(int point)
    {
        LoyaltyPoints = point;
    }

    public void AddPoint(int point)
    {
        LoyaltyPoints += point; 
    }

    public void ReducePoint(int point)
    {
        LoyaltyPoints -= point; 
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

    private void RaiseTierDowngradedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseTierUpgradedDomainEvent()
    {
        throw new NotImplementedException();
    }
}
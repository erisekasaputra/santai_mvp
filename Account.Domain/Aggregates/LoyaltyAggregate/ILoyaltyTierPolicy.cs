using Account.Domain.Enumerations;

namespace Account.Domain.Aggregates.LoyaltyAggregate;

public interface ILoyaltyTierPolicy
{
    LoyaltyTier DetermineTier(int loyaltyPoints);
}

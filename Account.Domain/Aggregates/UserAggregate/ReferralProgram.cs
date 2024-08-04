using Account.Domain.Extensions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.UserAggregate;

public class ReferralProgram : Entity, IAggregateRoot
{ 
    public string ReferralCode { get; private init; }
    public Guid? ReferredBy { get; private set; }
    public DateTime ReferralDate { get; private set; }
    public int RewardPoint { get; private set; }

    public ReferralProgram(Guid referredBy, DateTime referralDate, int rewardPoint)
    {
        Id = Guid.NewGuid();
        ReferredBy = referredBy;
        ReferralCode = UniqueIdGenerator.Generate(referredBy);
        ReferralDate = referralDate;
        RewardPoint = rewardPoint;
    } 
}
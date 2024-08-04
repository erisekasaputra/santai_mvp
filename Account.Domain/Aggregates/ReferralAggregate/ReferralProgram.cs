using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Extensions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.ReferralAggregate;

public class ReferralProgram : Entity, IAggregateRoot
{
    public string ReferralCode { get; private init; }

    public Guid UserId { get; private set; }

    public DateTime ReferralDate { get; private set; }

    public int RewardPoint { get; private set; }

    public User User { get; private set; }   

    public ReferralProgram(Guid userId, DateTime referralDate, int rewardPoint)
    {   
        UserId = userId;
        ReferralCode = UniqueIdGenerator.Generate(userId);
        ReferralDate = referralDate;
        RewardPoint = rewardPoint;
    }
}
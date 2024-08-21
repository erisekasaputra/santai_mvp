using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Extensions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.ReferralAggregate;

public class ReferralProgram : Entity, IAggregateRoot
{
    public string ReferralCode { get; private init; }

    public Guid UserId { get; private init; }

    public DateTime ValidDateUtc { get; private init; }

    public int RewardPoint { get; private set; }

    public BaseUser BaseUser { get; private set; }

    public ReferralProgram(Guid userId, DateTime validDateUtc, int rewardPoint)
    {   
        if (validDateUtc < DateTime.UtcNow)
        {
            throw new Exception("Referral valid date can not in the past");
        }

        UserId = userId != default ? userId : throw new ArgumentNullException(nameof(userId));
        ReferralCode = UniqueIdGenerator.Generate(userId); 
        ValidDateUtc = validDateUtc != default ? validDateUtc : throw new ArgumentNullException(nameof(validDateUtc));
        RewardPoint = rewardPoint > 0 ? rewardPoint : throw new ArgumentOutOfRangeException(nameof(rewardPoint));
    } 
}
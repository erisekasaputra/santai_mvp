using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.ReferredAggregate;

public class ReferredProgram : Entity, IAggregateRoot
{ 
    public Guid ReferrerId { get; private set; }  

    public Guid ReferredUserId { get; private set; } 

    public string ReferralCode { get; private set; }

    public DateTime ReferredDate { get; private set; }

    public ReferralStatus Status { get; private set; }

    public User User { get; private set; } // navigation properties

    public ReferredProgram(Guid referrerId, Guid referredUserId, string referralCode, DateTime referredDate)
    {
        ReferrerId = referrerId;
        ReferredUserId = referredUserId;
        ReferralCode = referralCode;
        ReferredDate = referredDate;
        Status = ReferralStatus.Pending;
    } 
}

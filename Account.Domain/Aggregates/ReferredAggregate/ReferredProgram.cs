using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.ReferredAggregate;

public class ReferredProgram : Entity, IAggregateRoot
{ 
    public Guid ReferrerId { get; set; }  
    public Guid ReferredUserId { get; set; }  
    public string ReferralCode { get; set; }
    public DateTime ReferredDate { get; set; }
    public ReferralStatus Status { get; set; } 

    public ReferredProgram(Guid referrerId, Guid referredUserId, string referralCode, DateTime referredDate)
    {
        ReferrerId = referrerId;
        ReferredUserId = referredUserId;
        ReferralCode = referralCode;
        ReferredDate = referredDate;
        Status = ReferralStatus.Pending;
    }
}

using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;

namespace Account.Domain.Aggregates.ReferredAggregate;

public class ReferredProgram : Entity, IAggregateRoot
{ 
    public Guid ReferrerId { get; private init; }  

    public Guid ReferredUserId { get; private init; } 

    public string ReferralCode { get; private init; }

    public DateTime ReferredDateUtc { get; private init; } 

    public ReferralStatus Status { get; private set; }

    public BaseUser BaseUser { get; private set; } // navigation properties

    public ReferredProgram(Guid referrerId, Guid referredUserId, string referralCode, DateTime referredDateUtc)
    { 
        ReferrerId = referrerId != default ? referrerId : throw new ArgumentNullException(nameof(referrerId));
        ReferredUserId = referredUserId != default ? referredUserId : throw new ArgumentNullException(nameof(referredUserId));
        ReferralCode = referralCode ?? throw new ArgumentNullException(nameof(referralCode));
        ReferredDateUtc = referredDateUtc; 
        Status = ReferralStatus.Pending;
    } 

    public void ConvertPoint()
    { 
        if (Status != ReferralStatus.Pending) 
        {
            throw new DomainException("Can not convert point once the point is converted");  
        } 

        Status = ReferralStatus.Converted;
        RaisePointConvertedDomainEvent();
    }

    private void RaisePointConvertedDomainEvent()
    { 
    }
}

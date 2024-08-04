using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Aggregates.UserAggregate;

public class RegularUser : User
{
    public PersonalInfo PersonalInfo { get; private set; }

    public ReferralProgram ReferralProgram { get; private set; }

    public LoyaltyProgram LoyaltyProgram { get; private set; }

    public string? DeviceId { get; private set; }

    public RegularUser() : base()
    {
        
    }

    public RegularUser(
        string username,
        string email,
        string phoneNumber,
        Address address,
        PersonalInfo personalInfo,
        string deviceId,
        int referralRewardPoint) : base(username, email, phoneNumber, address)
    {
        PersonalInfo = personalInfo ?? throw new ArgumentNullException(nameof(personalInfo));
        ReferralProgram = new ReferralProgram(Id, DateTime.UtcNow, referralRewardPoint);
        LoyaltyProgram = new LoyaltyProgram(Id, 0, Enumerations.LoyaltyTier.Basic);
        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseRegularUserCreatedDomainEvent();
    }

    protected override void UpdateEmail(string email)
    {
        base.UpdateEmail(email);

        RaiseEmailUpdatedDomainEvent();
    } 
    
    protected override void UpdatePhoneNumber(string phoneNumber)
    {
        base.UpdatePhoneNumber(phoneNumber);

        RaisePhoneNumberUpdatedDomainEvent();
    } 

    protected override void VerifyEmail()
    {
        base.VerifyEmail();

        RaiseEmailVerifiedDomainEvent();
    }  

    protected override void VerifyPhoneNumber()
    {
        base.VerifyPhoneNumber();

        RaisePhoneNumberVerifiedDomainEvent();
    } 

    public void ResetDeviceId()
    {
        if (DeviceId is null)
        {
            return;
        }

        DeviceId = null;

        RaiseDeviceIdResetDomainEvent();
    }  

    public void SetDeviceId(string deviceId)
    {
        if (deviceId is not null)
        {
            throw new DomainException("This account is already registered with another device");
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdSetDomainEvent();
    } 
    

    public void ForceSetDeviceId(string deviceId)
    {
        if (DeviceId == deviceId)
        {
            return;
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdForcedSetDomainEvent();
    }

    private void RaiseEmailUpdatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaisePhoneNumberUpdatedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseEmailVerifiedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaisePhoneNumberVerifiedDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdForcedSetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdSetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdResetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseRegularUserCreatedDomainEvent()
    {
        throw new NotImplementedException();
    }
}

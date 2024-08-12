using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Aggregates.UserAggregate;

public class RegularUser : User
{
    public PersonalInfo PersonalInfo { get; private set; } 

    public string? DeviceId { get; private set; }

    public RegularUser() : base()
    { 
    }

    public RegularUser(
        Guid identityId,
        string username,
        string email,
        string encryptedEmail,
        string phoneNumber,
        string encryptedPhoneNumber,
        Address address, 
        PersonalInfo personalInfo,
        string timeZoneId,
        string deviceId) : base(identityId, username, email, encryptedEmail, phoneNumber, encryptedPhoneNumber, address, timeZoneId)
    { 
        PersonalInfo = personalInfo ?? throw new ArgumentNullException(nameof(personalInfo));
        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseRegularUserCreatedDomainEvent(this);
    }

    public void Update(string timeZoneId, Address address, PersonalInfo personalInfo)
    {
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PersonalInfo = personalInfo ?? throw new ArgumentNullException(nameof(personalInfo));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Delete()
    {
        RaiseRegularUserDeletedDomainEvent(Id);
    }

    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        base.AddReferralProgram(referralRewardPoint, referralValidDate);
    } 

    public void ResetDeviceId()
    {
        if (DeviceId is null)
        {
            return;
        }

        DeviceId = null;

        RaiseDeviceIdResetDomainEvent(Id);
    }  

    public void SetDeviceId(string deviceId)
    {
        if (deviceId is not null)
        {
            throw new DomainException("This account is already registered with another device");
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdSetDomainEvent(Id, deviceId);
    }  

    public void ForceSetDeviceId(string deviceId)
    {
        if (DeviceId == deviceId)
        {
            return;
        }

        DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

        RaiseDeviceIdForcedSetDomainEvent(Id, deviceId);
    }
     
    private void RaiseDeviceIdForcedSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdForcedSetDomainEvent(id, deviceId));
    }

    private void RaiseDeviceIdSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdSetDomainEvent(id, deviceId));
    }

    private void RaiseDeviceIdResetDomainEvent(Guid id)
    {
        AddDomainEvent(new DeviceIdResetDomainEvent(id));
    }

    private void RaiseRegularUserCreatedDomainEvent(RegularUser user)
    {
        AddDomainEvent(new RegularUserCreateDomainEvent(user));
    }

    private void RaiseRegularUserDeletedDomainEvent(Guid id)
    {
        AddDomainEvent(new RegularUserDeletedDomainEvent(id));
    }
}

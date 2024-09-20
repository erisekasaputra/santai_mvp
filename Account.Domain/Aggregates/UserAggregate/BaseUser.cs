using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Events; 
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using Core.Exceptions;

namespace Account.Domain.Aggregates.UserAggregate;

public abstract class BaseUser : Entity, IAggregateRoot
{     
    public string Name { get; set; }
    public string? HashedEmail { get; private set; } 
    public string? EncryptedEmail { get; private set; } 
    public bool IsEmailVerified { get; private set; } 
    public string? NewHashedEmail { get; private set; } 
    public string? NewEncryptedEmail { get; private set; } 
    public string? HashedPhoneNumber { get; private set; } 
    public string? EncryptedPhoneNumber { get; private set; } 
    public bool IsPhoneNumberVerified { get; private set; } 
    public string? NewHashedPhoneNumber { get; private set; } 
    public string? NewEncryptedPhoneNumber {  get; private set; } 
    public DateTime CreatedAtUtc { get; private init; } 
    public DateTime UpdatedAtUtc { get; protected set; } 
    public AccountStatus AccountStatus { get; private set; } 
    public Address Address { get; protected set; } 
    public LoyaltyProgram LoyaltyProgram { get; private set; } 
    public ReferralProgram? ReferralProgram { get; private set; } 
    public ICollection<ReferredProgram>? ReferredPrograms { get; private set; }  
    public ICollection<Fleet>? Fleets { get; private set; } 
    public string TimeZoneId {  get; set; }
    public ICollection<string> DeviceIds { get; private set; } = [];
    protected BaseUser()
    {
        Address = null!;
        LoyaltyProgram = null!; 
        TimeZoneId = null!;
        Name = null!; 
    }

    public BaseUser(
        string name,
        string? hashedEmail,
        string? encryptedEmail,
        string hashedPhoneNumber,
        string encryptedPhoneNumber,
        Address address,
        string timeZoneId,
        string deviceId,
        bool isEmailVerified = false,
        bool isPhoneNumberVerified = false)
    {
        Name = name;
        HashedEmail = hashedEmail;
        EncryptedEmail = encryptedEmail;
        HashedPhoneNumber = hashedPhoneNumber ?? throw new ArgumentNullException(nameof(hashedPhoneNumber));
        EncryptedPhoneNumber = encryptedPhoneNumber ?? throw new ArgumentNullException(nameof(encryptedPhoneNumber));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
        AccountStatus = AccountStatus.Active;

        if (!isPhoneNumberVerified)
        {
            NewHashedPhoneNumber = hashedPhoneNumber;
            NewEncryptedPhoneNumber = encryptedPhoneNumber;
        }

        if (hashedEmail is not null)
        {
            NewHashedEmail = hashedEmail;
            NewEncryptedEmail = encryptedEmail;
        }

        IsEmailVerified = isEmailVerified;
        IsPhoneNumberVerified = isPhoneNumberVerified;

        LoyaltyProgram = new LoyaltyProgram(Id, 0);

        DeviceIds ??= [];
        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            DeviceIds.Add(deviceId);
        }
    }

    public void ResetPhoneNumber()
    {
        if (NewHashedPhoneNumber == HashedPhoneNumber)
        {
            NewHashedPhoneNumber = null;
            NewEncryptedPhoneNumber = null;
            IsPhoneNumberVerified = false;
        }

        HashedPhoneNumber = null;
        EncryptedPhoneNumber = null;
        IsPhoneNumberVerified = false;
    }

    public void ResetEmail()
    { 
        HashedEmail = null;
        EncryptedEmail = null;
        IsEmailVerified = false;
    }

    public virtual void AddDeviceId(string deviceId)
    {
        if (DeviceIds.Contains(deviceId) || string.IsNullOrWhiteSpace(deviceId))
        {
            return;
        }

        DeviceIds ??= [];
        DeviceIds.Add(deviceId);
    }

    public virtual void RemoveDeviceId(string deviceId)
    {
        DeviceIds ??= [];
        DeviceIds.Remove(deviceId);  
    }

    public virtual void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(referralRewardPoint, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(referralValidDate, 0); 

        ReferralProgram = new ReferralProgram(Id, DateTime.UtcNow.AddMonths(referralValidDate), referralRewardPoint);

        RaiseReferralProgramAddedDomainEvent(ReferralProgram);
    }    

    public virtual void UpdateEmail(string email, string encryptedEmail)
    {
        NewHashedEmail = email;
        NewEncryptedEmail = encryptedEmail;
        IsEmailVerified = false; 
        UpdatedAtUtc = DateTime.UtcNow;
        RaiseEmailUpdatedDomainEvent(Id, HashedEmail, email, EncryptedEmail, encryptedEmail);
    } 

    public virtual void UpdatePhoneNumber(string phoneNumber, string encryptedPhoneNumber)
    {
        NewHashedPhoneNumber = phoneNumber;
        NewEncryptedPhoneNumber = encryptedPhoneNumber;
        IsPhoneNumberVerified = false;    
        UpdatedAtUtc = DateTime.UtcNow; 
        RaisePhoneNumberUpdatedDomainEvent(Id, HashedPhoneNumber, phoneNumber, EncryptedPhoneNumber, encryptedPhoneNumber);
    } 

    public virtual void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(NewHashedEmail) || string.IsNullOrWhiteSpace(NewEncryptedEmail))
        {
            throw new DomainException("New email is not set, can not verify email");
        }

        HashedEmail = NewHashedEmail;
        EncryptedEmail = NewEncryptedEmail;
        NewHashedEmail = null;
        NewEncryptedEmail = null;
        IsEmailVerified = true;

        RaiseEmailVerifiedDomainEvent(Id, HashedEmail, EncryptedEmail);
    }  

    public virtual void VerifyPhoneNumber()
    {
        if (string.IsNullOrWhiteSpace(NewHashedPhoneNumber) || string.IsNullOrWhiteSpace(NewEncryptedPhoneNumber)) 
        {
            throw new DomainException("New phone number is not set, can not vefify phone number");
        }

        HashedPhoneNumber = NewHashedPhoneNumber;
        EncryptedPhoneNumber = NewEncryptedPhoneNumber;
        NewHashedPhoneNumber = null;
        NewEncryptedPhoneNumber = null;
        IsPhoneNumberVerified = true;

        RaisePhoneNumberVerifiedDomainEvent(Id, HashedPhoneNumber, EncryptedPhoneNumber);
    } 

    private void RaiseReferralProgramAddedDomainEvent(ReferralProgram referralProgram)
    {
        AddDomainEvent(new ReferralProgramAddedDomainEvent(referralProgram));
    }

    private void RaiseEmailUpdatedDomainEvent(Guid id, string? oldEmail, string newEmail, string? oldEncryptedEmail, string newEncryptedEmail)
    {
        AddDomainEvent(new EmailUpdatedDomainEvent(id, oldEmail, newEmail, oldEncryptedEmail, newEncryptedEmail));
    }

    private void RaisePhoneNumberUpdatedDomainEvent(Guid id, string? oldPhoneNumber, string newPhoneNumber, string? oldEncryptedPhoneNumber, string newEncryptedPhoneNumber)
    {
        AddDomainEvent(new PhoneNumberUpdatedDomainEvent(id, oldPhoneNumber, newPhoneNumber, oldEncryptedPhoneNumber, newEncryptedPhoneNumber));
    }

    private void RaiseEmailVerifiedDomainEvent(Guid id, string email, string encryptedEmail)
    {
        AddDomainEvent(new EmailVerifiedDomainEvent(id, email, encryptedEmail));
    }

    private void RaisePhoneNumberVerifiedDomainEvent(Guid id, string phoneNumber, string encryptedPhoneNumber)
    {
        AddDomainEvent(new PhoneNumberVerifiedDomainEvent(id, phoneNumber, encryptedPhoneNumber));
    }
}

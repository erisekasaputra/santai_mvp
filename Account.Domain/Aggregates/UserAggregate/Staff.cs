using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class Staff : Entity, IAggregateRoot
{ 
    public Guid BusinessUserId { get; private init; }
    public string BusinessUserCode { get; private init; }
    public BusinessUser? BusinessUser { get; set; } // Only for navigation properties, does not have to be instantiated 
    public string? HashedPhoneNumber { get; private set; } 
    public string? EncryptedPhoneNumber { get; private set; }
    public string? NewHashedPhoneNumber { get; private set; } 
    public string? NewEncryptedPhoneNumber { get; private set; }
    public bool IsPhoneNumberVerified { get; private set; }  
    public string? HashedEmail { get; private set; } 
    public string? EncryptedEmail {  get; private set; } 
    public string? NewHashedEmail { get; private set; } 
    public string? NewEncryptedEmail {  get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string Name { get; private set; }
    public string? DeviceId { get; private set; } 
    public Address Address { get; private set; }
    public string TimeZoneId { get; private set; }
    public ICollection<Fleet>? Fleets { get; private set; } 

    public Staff()
    {
        Address = null!;
        BusinessUserCode = null!;
        Name = null!;
        TimeZoneId = null!;
    } 
    
    public Staff( 
        Guid businessUserId,
        string businessUserCode,
        string? hashedEmail,
        string? encryptedEmail,
        string hashedPhoneNumber,
        string encryptedPhoneNumber,
        string name,
        Address address,
        string timeZoneId,
        string? deviceId)
    { 
        BusinessUserId = businessUserId != default ? businessUserId : throw new ArgumentNullException(nameof(businessUserId)); 
        BusinessUserCode = businessUserCode ?? throw new ArgumentNullException(nameof(businessUserCode));   
        Name = name ?? throw new ArgumentNullException(nameof(name)); 
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId)); 
        Address = address ?? throw new ArgumentNullException(nameof(address));  
        DeviceId = deviceId ?? null; 

        HashedPhoneNumber = hashedPhoneNumber ?? throw new ArgumentNullException(nameof(hashedPhoneNumber));
        NewHashedPhoneNumber = hashedPhoneNumber;
        EncryptedPhoneNumber = encryptedPhoneNumber ?? throw new ArgumentNullException(nameof(hashedPhoneNumber));
        NewEncryptedPhoneNumber = encryptedPhoneNumber; 

        if (hashedEmail is not null)
        {
            HashedEmail = hashedEmail;
            EncryptedEmail = encryptedEmail;
        }

        IsEmailVerified = false; 
        IsPhoneNumberVerified = false; 
    }

    public void ResetPhoneNumber()
    {
        HashedPhoneNumber = null;
        EncryptedPhoneNumber = null;
        IsPhoneNumberVerified = false;
    }

    public void Update(string name, Address address, string timeZoneId)
    { 
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address)); 
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
    } 

    public void UpdateEmail(string email, string encryptedEmail)
    {
        NewHashedEmail = email;
        NewEncryptedEmail = encryptedEmail;
        IsEmailVerified = false;

        RaiseEmailUpdatedDomainEvent(Id, HashedEmail, email, EncryptedEmail, encryptedEmail);
    }

    public void UpdatePhoneNumber(string phoneNumber, string encryptedPhoneNumber)
    { 
        NewHashedPhoneNumber = phoneNumber;
        IsPhoneNumberVerified = false;

        RaisePhoneNumberUpdatedDomainEvent(Id, HashedPhoneNumber, phoneNumber, EncryptedPhoneNumber, encryptedPhoneNumber);
    }

    public void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(NewHashedEmail) || string.IsNullOrWhiteSpace(NewEncryptedEmail))
        {
            throw new DomainException("New email is not set, can not verify email");
        }

        HashedEmail = NewHashedEmail;
        EncryptedEmail = NewEncryptedEmail;
        IsEmailVerified = true;
        NewHashedEmail = null;
        NewEncryptedEmail = null;

        RaiseEmailVerifiedDomainEvent(Id, HashedEmail, EncryptedEmail);
    }

    public void VerifyPhoneNumber()
    {
        if (string.IsNullOrWhiteSpace(NewHashedPhoneNumber) || string.IsNullOrWhiteSpace(NewEncryptedPhoneNumber))
        {
            throw new DomainException("New phone number is not set, can not phone number");
        }

        HashedPhoneNumber = NewHashedPhoneNumber;
        EncryptedPhoneNumber = NewEncryptedPhoneNumber;
        IsPhoneNumberVerified = true;
        NewHashedPhoneNumber = null;
        NewEncryptedPhoneNumber = null;

        RaisePhoneNumberVerifiedDomainEvent(Id, HashedPhoneNumber, EncryptedPhoneNumber);
    }

    public void ResetDeviceId()
    {
        if (string.IsNullOrWhiteSpace(DeviceId))
        {
            return;  
        }

        DeviceId = null;

        RaiseDeviceIdResetDomainEvent(Id);
    }

    public void SetDeviceId(string deviceId)
    {
        if (!string.IsNullOrWhiteSpace(DeviceId))
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

    private void RaiseDeviceIdResetDomainEvent(Guid id)
    {
        AddDomainEvent(new DeviceIdResetDomainEvent(id));
    }

    private void RaiseDeviceIdSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdSetDomainEvent(id, deviceId));
    }

    private void RaiseDeviceIdForcedSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdForcedSetDomainEvent(id, deviceId));
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

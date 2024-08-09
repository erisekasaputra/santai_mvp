using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public class Staff : Entity, IAggregateRoot
{
    public string Username { get; private init; } 
    public Guid BusinessUserId { get; private init; } 
    public string BusinessUserCode { get; private init; } 
    public BusinessUser BusinessUser { get; set; } // Only for navigation properties, does not have to be instantiated 
    public string PhoneNumber { get; private set; } 
    public string? NewPhoneNumber { get; private set; } 
    public bool IsPhoneNumberVerified { get; private set; }  
    public string Email { get; private set; } 
    public string? NewEmail { get; private set; } 
    public bool IsEmailVerified { get; private set; } 
    public string Name { get; private set; }  
    public string? DeviceId { get; private set; } 
    public Address Address { get; private set; }  
    public string TimeZoneId { get; private set; } 
    public Staff()
    {
        Username = string.Empty;
        BusinessUserCode = string.Empty;
        PhoneNumber = string.Empty;
        NewPhoneNumber = string.Empty;
        Email = string.Empty;
        NewEmail = string.Empty;
        Name = string.Empty;
        TimeZoneId = string.Empty;
        Address = new Address();
        BusinessUser = new BusinessUser(); 
    }

    public Staff(
        Guid businessUserId,
        string businessUserCode,
        string username,
        string email,
        string phoneNumber,
        string name,
        Address address,
        string timeZoneId,
        string? deviceId)
    {
        BusinessUserId = businessUserId != default ? businessUserId : throw new ArgumentNullException(nameof(businessUserId)); 
        BusinessUserCode = businessUserCode ?? throw new ArgumentNullException(nameof(businessUserCode));  
        Username = username; 
        Email = email ?? throw new ArgumentNullException( nameof(email)); 
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber)); 
        Name = name ?? throw new ArgumentNullException(nameof(name)); 
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId)); 
        Address = address ?? throw new ArgumentNullException(nameof(address));  
        DeviceId = deviceId ?? null; 
        NewEmail = email; 
        NewPhoneNumber = phoneNumber;  
        IsEmailVerified = false; 
        IsPhoneNumberVerified = false;
        BusinessUser = new BusinessUser();
    }

    public void UpdateAddress(
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string city,
        string state,
        string postalCode,
        string country)
    {   
        ArgumentNullException.ThrowIfNullOrWhiteSpace(addressLine1);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(city);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(state);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(country);

        Address = new Address(addressLine1, addressLine2, addressLine3, city, state, postalCode, country);
    }

    public void UpdateEmail(string email)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

        NewEmail = email;
        IsEmailVerified = false;

        RaiseEmailUpdatedDomainEvent(Id, Email, email);
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);

        NewPhoneNumber = phoneNumber;
        IsPhoneNumberVerified = false;

        RaisePhoneNumberUpdatedDomainEvent(Id, PhoneNumber, phoneNumber);
    }

    public void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(NewEmail))
        {
            throw new DomainException("New email is not set, can not verify email");
        }

        Email = NewEmail;
        IsEmailVerified = true;
        NewEmail = null;

        RaiseEmailVerifiedDomainEvent(Id, Email);
    }

    public void VerifyPhoneNumber()
    {
        if (string.IsNullOrWhiteSpace(NewPhoneNumber))
        {
            throw new DomainException("New phone number is not set, can not phone number");
        }

        PhoneNumber = NewPhoneNumber;
        IsPhoneNumberVerified = true;
        NewPhoneNumber = null;

        RaisePhoneNumberVerifiedDomainEvent(Id, PhoneNumber);
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

        RaiseDeviceIdForcedSetDomainEvent(Id);
    }

    private void RaiseDeviceIdResetDomainEvent(Guid id)
    {
        AddDomainEvent(new DeviceIdResetDomainEvent(id));
    }

    private void RaiseDeviceIdSetDomainEvent(Guid id, string deviceId)
    {
        AddDomainEvent(new DeviceIdSetDomainEvent(id, deviceId));
    }

    private void RaiseDeviceIdForcedSetDomainEvent(Guid id)
    {
        AddDomainEvent(new DeviceIdForcedResetDomainEvent(id));
    }

    private void RaiseEmailUpdatedDomainEvent(Guid id, string oldEmail, string newEmail)
    {
        AddDomainEvent(new EmailUpdatedDomainEvent(id, oldEmail, newEmail));
    }

    private void RaisePhoneNumberUpdatedDomainEvent(Guid id, string oldPhoneNumber, string newPhoneNumber)
    {
        AddDomainEvent(new PhoneNumberUpdatedDomainEvent(id, oldPhoneNumber, newPhoneNumber));
    }

    private void RaiseEmailVerifiedDomainEvent(Guid id, string email)
    {
        AddDomainEvent(new EmailVerifiedDomainEvent(id, email));
    }

    private void RaisePhoneNumberVerifiedDomainEvent(Guid id, string phoneNumber)
    {
        AddDomainEvent(new PhoneNumberVerifiedDomainEvent(id, phoneNumber));
    }
}

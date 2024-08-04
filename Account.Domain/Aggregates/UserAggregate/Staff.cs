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

    public Address? Address { get; private set; }

    public Staff()
    {

    }

    public Staff(
        Guid businessUserId,
        string businessUserCode,
        string username,
        string email,
        string phoneNumber,
        string name,
        Address address,
        string? deviceId)
    {
        BusinessUserId = businessUserId != default ? businessUserId : throw new ArgumentNullException(nameof(businessUserId));
        
        BusinessUserCode = businessUserCode ?? throw new ArgumentNullException(nameof(businessUserCode));
        
        Username = username;
        
        Email = email ?? throw new ArgumentNullException( nameof(email));
        
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        
        Name = name ?? throw new ArgumentNullException(nameof(name));
        
        Address = address;
        
        DeviceId = deviceId ?? null;

        NewEmail = email;
        
        NewPhoneNumber = phoneNumber;
        
        IsEmailVerified = false;
        
        IsPhoneNumberVerified = false; 
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

        RaiseEmailUpdatedDomainEvent();
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);

        NewPhoneNumber = phoneNumber;
        IsPhoneNumberVerified = false;

        RaisePhoneNumberUpdatedDomainEvent();
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

        RaiseEmailVerifiedDomainEvent();
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

    private void RaiseDeviceIdResetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdSetDomainEvent()
    {
        throw new NotImplementedException();
    }

    private void RaiseDeviceIdForcedSetDomainEvent()
    {
        throw new NotImplementedException();
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
}

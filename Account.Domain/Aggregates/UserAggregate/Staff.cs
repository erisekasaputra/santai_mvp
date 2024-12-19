using Account.Domain.Aggregates.FleetAggregate;
using Account.Domain.Events; 
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;
using Core.Exceptions;

namespace Account.Domain.Aggregates.UserAggregate;

public class Staff : Entity, IAggregateRoot
{  
    public Guid BusinessUserId { get; private init; }
    public string BusinessUserCode { get; private init; } 
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
    public string ImageUrl { get; private set; }
    public Address Address { get; private set; }
    public string TimeZoneId { get; private set; }
    public string Password { get; private set; }
    public ICollection<Fleet>? Fleets { get; private set; } 
    public Staff()
    {
        Password = string.Empty;
        ImageUrl = string.Empty;
        Address = null!;
        BusinessUserCode = null!;
        Name = null!;
        TimeZoneId = null!;
    } 
    
    public Staff( 
        Guid businessUserId,
        string businessUserCode, 
        string hashedPhoneNumber,
        string encryptedPhoneNumber,
        string name,
        string imageUrl,
        Address address,
        string timeZoneId, 
        string password, 
        bool raiseCreatedEvent = true)
    { 
        BusinessUserId = businessUserId != default ? businessUserId : throw new ArgumentNullException(nameof(businessUserId)); 
        BusinessUserCode = businessUserCode ?? throw new ArgumentNullException(nameof(businessUserCode));   
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ImageUrl = imageUrl;
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId)); 
        Address = address ?? throw new ArgumentNullException(nameof(address));
           
        HashedPhoneNumber = hashedPhoneNumber ?? throw new ArgumentNullException(nameof(hashedPhoneNumber)); 
        EncryptedPhoneNumber = encryptedPhoneNumber ?? throw new ArgumentNullException(nameof(hashedPhoneNumber)); 

        IsEmailVerified = false; 
        IsPhoneNumberVerified = true;
        Password = password;

        if (raiseCreatedEvent)
        {
            RaiseStaffCreatedDomainEvent();
        }
    }

    public void ResetPhoneNumber()
    {
        HashedPhoneNumber = null;
        EncryptedPhoneNumber = null;
        IsPhoneNumberVerified = false;
    }

    public void Update(string name, string imageUrl, Address address, string timeZoneId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ImageUrl = imageUrl;
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

    private void RaiseStaffCreatedDomainEvent()
    {
        AddDomainEvent(new StaffCreatedDomainEvent(this));
    }
}

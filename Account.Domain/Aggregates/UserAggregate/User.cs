using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects; 

namespace Account.Domain.Aggregates.UserAggregate;

public abstract class User : Entity, IAggregateRoot
{
    public Guid IdentityId { get; private init; }

    public string Username { get; private init; }

    public string Email { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public string? NewEmail { get; private set; }

    public string PhoneNumber { get; private set; }

    public bool IsPhoneNumberVerified { get; private set; }

    public string? NewPhoneNumber { get; private set; }

    public DateTime CreatedAtUtc { get; private init; }

    public DateTime UpdatedAtUtc { get; private set; }

    public AccountStatus AccountStatus { get; private set; }

    public Address Address { get; protected set; }
     
    public ReferralProgram? ReferralProgram { get; protected set; }

    public LoyaltyProgram? LoyaltyProgram { get; protected set; }
     
    public ICollection<ReferredProgram>? ReferredPrograms { get; protected set; }

    public string TimeZoneId {  get; set; }  

    protected User()
    {

    }

    public User(Guid identityId, string username, string email, string phoneNumber, Address address, string timeZoneId)
    {
        IdentityId = identityId != default ? identityId : throw new ArgumentNullException(nameof(identityId));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId)); 
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
        AccountStatus = AccountStatus.Active;

        NewEmail = Email;
        NewPhoneNumber = PhoneNumber;
        IsEmailVerified = false;
        IsPhoneNumberVerified = false; 
         
        LoyaltyProgram = new LoyaltyProgram(Id, 0); 
    }

    public virtual void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(referralRewardPoint, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(referralValidDate, 0); 

        ReferralProgram = new ReferralProgram(Id, DateTime.UtcNow.AddMonths(referralValidDate), referralRewardPoint);

        RaiseReferralProgramAddedDomainEvent(ReferralProgram);
    } 

    public virtual void UpdateEmail(string email)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

        NewEmail = email;
        IsEmailVerified = false;

        UpdatedAtUtc = DateTime.UtcNow;

        RaiseEmailUpdatedDomainEvent(Id, Email, NewEmail);
    } 

    public virtual void UpdatePhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);

        NewPhoneNumber = phoneNumber;
        IsPhoneNumberVerified = false; 

        UpdatedAtUtc = DateTime.UtcNow;

        RaisePhoneNumberUpdatedDomainEvent(Id, PhoneNumber, NewPhoneNumber);
    } 

    public virtual void VerifyEmail()
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

    public virtual void VerifyPhoneNumber()
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

    private void RaiseReferralProgramAddedDomainEvent(ReferralProgram referralProgram)
    {
        AddDomainEvent(new ReferralProgramAddedDomainEvent(referralProgram));
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

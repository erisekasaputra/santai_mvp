using Account.Domain.Aggregates.LoyaltyAggregate;
using Account.Domain.Aggregates.ReferralAggregate;
using Account.Domain.Aggregates.ReferredAggregate;
using Account.Domain.Enumerations;
using Account.Domain.Exceptions;
using Account.Domain.SeedWork;
using Account.Domain.ValueObjects;

namespace Account.Domain.Aggregates.UserAggregate;

public abstract class User : Entity, IAggregateRoot
{
    public Guid IdentityId { get; set; }

    public string Username { get; private init; }

    public string Email { get; private set; }

    public bool IsEmailVerified { get; private set; }
     
    private string? NewEmail { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsPhoneNumberVerified { get; private set; }

    private string? NewPhoneNumber { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; set; }

    public AccountStatus AccountStatus { get; set; }

    public Address Address { get; set; }

    public ReferralProgram ReferralProgram { get; private set; }

    public LoyaltyProgram LoyaltyProgram { get; private set; }

    public ICollection<ReferredProgram> ReferredPrograms { get; set; }

    public User()
    {
    }

    public User(Guid identityId, string username, string email, string phoneNumber, Address address, int referralRewardPoint)
    {
        IdentityId = identityId != default ? identityId : throw new ArgumentNullException(nameof(identityId));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AccountStatus = AccountStatus.Active;

        NewEmail = Email;
        NewPhoneNumber = PhoneNumber;
        IsEmailVerified = false;
        IsPhoneNumberVerified = false;

        ReferralProgram = new ReferralProgram(Id, DateTime.UtcNow, referralRewardPoint);
        LoyaltyProgram = new LoyaltyProgram(Id, 0, Enumerations.LoyaltyTier.Basic);
    }

    protected virtual void UpdateEmail(string email)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

        NewEmail = email;
        IsEmailVerified = false;

        UpdatedAt = DateTime.UtcNow;
    }

    protected virtual void UpdatePhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber);

        NewPhoneNumber = phoneNumber;
        IsPhoneNumberVerified = false; 

        UpdatedAt = DateTime.UtcNow;
    }

    protected virtual void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(NewEmail))
        {
            throw new DomainException("New email is not set, can not verify email");
        }

        Email = NewEmail;
        IsEmailVerified = true;
        NewEmail = null;
    }

    protected virtual void VerifyPhoneNumber()
    {
        if (string.IsNullOrWhiteSpace(NewPhoneNumber))
        {
            throw new DomainException("New phone number is not set, can not phone number");
        }
         
        PhoneNumber = NewPhoneNumber;
        IsPhoneNumberVerified = true;
        NewPhoneNumber = null;
    }
}

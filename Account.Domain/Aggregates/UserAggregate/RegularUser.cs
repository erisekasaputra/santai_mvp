using Account.Domain.Events; 
using Account.Domain.ValueObjects; 
using Core.Extensions;

namespace Account.Domain.Aggregates.UserAggregate;

public class RegularUser : BaseUser
{   
    public PersonalInfo PersonalInfo { get; private set; }  
    public RegularUser() : base()
    {
        PersonalInfo = null!;
        Name = null!;
    }

    public RegularUser(
        Guid identityId, 
        string? email,
        string? encryptedEmail,
        string phoneNumber,
        string encryptedPhoneNumber,
        Address address, 
        PersonalInfo personalInfo,
        string timeZoneId ) : base(
            $"{personalInfo.FirstName} {personalInfo.MiddleName} {personalInfo.LastName}".CleanAndLowering(),
            email,
            encryptedEmail,
            phoneNumber,
            encryptedPhoneNumber,
            address,
            timeZoneId )
    { 
        Id = identityId;
        PersonalInfo = personalInfo ?? throw new ArgumentNullException(nameof(personalInfo)); 

        RaiseRegularUserCreatedDomainEvent(this);
    }

    public void Update(string timeZoneId, Address address, PersonalInfo personalInfo)
    {
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PersonalInfo = personalInfo ?? throw new ArgumentNullException(nameof(personalInfo));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfilePicture(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        PersonalInfo.ProfilePictureUrl = path;
    }

    public void Delete()
    {
        RaiseRegularUserDeletedDomainEvent(Id);
    }

    public override void AddReferralProgram(int referralRewardPoint, int referralValidDate)
    {
        base.AddReferralProgram(referralRewardPoint, referralValidDate);
    } 
     

    private void RaiseRegularUserCreatedDomainEvent(RegularUser user)
    {
        AddDomainEvent(new RegularUserCreatedDomainEvent(user));
    }

    private void RaiseRegularUserDeletedDomainEvent(Guid id)
    {
        AddDomainEvent(new RegularUserDeletedDomainEvent(id));
    }
}

namespace Notification.Worker.Domain;

public class UserProfile
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public required string PhoneNumber { get; set; }
    public ICollection<IdentityProfile> Profiles { get; set; } = [];

    public UserProfile()
    {
        
    }
    public UserProfile(
        Guid id,
        string? email,
        string phoneNumber)
    {
        Id = id;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void AddUserProfile(IdentityProfile profile)
    {
        if (Profiles.Any(x => x.DeviceToken == profile.DeviceToken))
        {
            return;
        }
        Profiles.Add(profile);
    }

    public void RemoveUserProfile(IdentityProfile profile) 
    {
        if (!Profiles.Any(x => x.DeviceToken == profile.DeviceToken))
        {
            return;
        } 
        Profiles.Remove(profile);
    }
    public void SetArn(string deviceToken, string arn)
    {
        var identityProfile = Profiles.Where(x => x.DeviceToken == deviceToken).FirstOrDefault(); 
        if (identityProfile is null)
        {
            return;
        }

        identityProfile.SetArn(arn);
    }    
}

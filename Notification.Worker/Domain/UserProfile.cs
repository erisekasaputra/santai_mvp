namespace Notification.Worker.Domain;

public class UserProfile
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public ICollection<IdentityProfile> Profiles { get; set; } = []; 

    public UserProfile()
    {
        PhoneNumber = string.Empty;
    }
    public UserProfile(
        Guid id,
        string phoneNumber,
        string? email)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        Email = email; 
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

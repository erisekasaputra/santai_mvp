namespace Notification.Worker.Domain;

public class IdentityProfile
{
    public required string DeviceToken { get; set; }
    public string? Arn { get; set; } 
    public IdentityProfile(string deviceToken, string? arn)
    {
        DeviceToken = deviceToken ?? throw new Exception("Device token can not be empty");
        Arn = arn;
    }

    public void SetArn(string arn)
    {
        Arn = arn;
    }
}


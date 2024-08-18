namespace Identity.API.Domain.Entities;

public class Otp
{
    public required string HashedPhoneNumber { get; set; }
    public long LockTimeUnix { get; set; }
    public required string HashedToken { get; set; }
}

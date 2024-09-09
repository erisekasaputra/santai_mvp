namespace Core.Models;

public class RefreshToken
{
    public required string Token { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    public required string UserId { get; set; }
}

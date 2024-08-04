namespace Identity.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string HashPassword { get; set; }
    public string SaltPassword { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
     
    public DateTime CreatedAt { get; init; }
}

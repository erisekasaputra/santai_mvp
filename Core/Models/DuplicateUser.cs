using Core.Enumerations;

namespace Core.Models;

public class DuplicateUser
{
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public UserType UserType { get; set; }
    public DuplicateUser(Guid id, string? phoneNumber, UserType userType)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        UserType = userType;
    }

    public DuplicateUser()
    {

    }
}

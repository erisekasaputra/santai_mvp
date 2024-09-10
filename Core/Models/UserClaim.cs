using Core.Enumerations;

namespace Core.Models;

public class UserClaim
{
    public Guid Sub { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public UserType CurrentUserType { get; set; }
    public string? BusinessCode { get; set; } 
    public string? UserRole { get; set; }    

    public UserClaim(
        Guid sub,
        string phoneNumber,
        string? email,
        UserType userType,
        string? businessCode,
        string? userRole)
    {
        if (userType == UserType.StaffUser || userType == UserType.BusinessUser)
        {
            ArgumentNullException.ThrowIfNull(businessCode);
        }

        Sub = sub;
        PhoneNumber = phoneNumber;
        Email = email;
        CurrentUserType = userType;
        BusinessCode = businessCode;
        UserRole = userRole;
    }
}

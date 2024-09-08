

using Core.Enumerations;

namespace Order.API.Applications.Models;


public class UserClaim
{
    public Guid Sub { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public UserType CurrentUserType { get; set; }
    public string? BusinessCode { get; set; }

    public UserClaim(Guid sub, string phoneNumber, string? email, UserType userType, string? businessCode)
    {
        if (userType == UserType.StaffUser || userType == UserType.BusinessUser)
        {
            ArgumentNullException.ThrowIfNull(businessCode);
        }

        this.Sub = sub;
        this.PhoneNumber = phoneNumber;
        this.Email = email;
        this.CurrentUserType = userType;
        this.BusinessCode = businessCode;
    }
}
namespace Identity.API.Applications.Dto;

public class LoginUserRequest
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RegionCode { get; set; }
    public string? ReturnUrl { get; set; }

    public LoginUserRequest( 
       string phoneNumber,
       string password,
       string regionCode,
       string returnUrl)
    { 
        PhoneNumber = phoneNumber.Trim();
        Password = password.Trim();
        RegionCode = regionCode.Trim();
        ReturnUrl = returnUrl.Trim();
    }
}

using Core.Enumerations; 

namespace Identity.API.Applications.Dto;

public class RegisterUserRequest
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RegionCode { get; set; }
    public UserType UserType { get; set; }
    public string? GoogleIdToken { get; set; }
    public string? ReturnUrl { get; set; }

    public RegisterUserRequest(
        string phoneNumber,
        string password,
        string regionCode,
        UserType userType, 
        string? googleIdToken,
        string? returnUrl)
    {
        PhoneNumber = phoneNumber.Trim();
        Password = password.Trim();
        RegionCode = regionCode.Trim();
        UserType = userType;
        GoogleIdToken = googleIdToken;
        ReturnUrl = returnUrl;
    }
}

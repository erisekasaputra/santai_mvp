namespace Identity.API.Applications.Dto;

public class LoginStaffRequest
{
    public required string BusinessCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string RegionCode { get; set; }
    public string? ReturnUrl { get; set; }

    public LoginStaffRequest(
        string businessCode,
        string phoneNumber,
        string password,
        string regionCode,
        string? returnUrl)
    {
        BusinessCode = businessCode.Trim();
        PhoneNumber = phoneNumber.Trim();
        Password = password.Trim();
        RegionCode = regionCode.Trim();
        ReturnUrl = returnUrl?.Trim();
    }
}
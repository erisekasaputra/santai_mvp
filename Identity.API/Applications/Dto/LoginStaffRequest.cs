namespace Identity.API.Applications.Dto;

public class LoginStaffRequest
{
    public required string BusinessCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public required string RegionCode { get; set; }
    public string? ReturnUrl { get; set; }
}
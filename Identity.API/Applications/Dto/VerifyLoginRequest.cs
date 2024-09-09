namespace Identity.API.Applications.Dto;

public class VerifyLoginRequest
{
    public required string Token { get; set; }
    public required string PhoneNumber { get; set; }
}

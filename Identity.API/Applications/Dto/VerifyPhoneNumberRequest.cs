namespace Identity.API.Applications.Dto;

public class VerifyPhoneNumberRequest
{
    public required string PhoneNumber { get; set; }
    public required string Token { get; set; }

    public VerifyPhoneNumberRequest(string phoneNumber, string token)
    {
        PhoneNumber = phoneNumber;
        Token = token;
    }
}

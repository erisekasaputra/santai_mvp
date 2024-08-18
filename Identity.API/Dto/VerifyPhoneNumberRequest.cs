namespace Identity.API.Dto;

public class VerifyPhoneNumberRequest
{
    public string PhoneNumber { get; set; }
    public string Token { get; set; }   

    public VerifyPhoneNumberRequest(string phoneNumber, string token)
    {
        PhoneNumber = phoneNumber;
        Token = token;
    }       
}

namespace Identity.API.Applications.Dto;

public class VerifyLoginRequest
{
    public string Token { get; set; }
    public string PhoneNumber { get; set; }
    public string DeviceId { get; set; }

    public VerifyLoginRequest(
        string token,
        string phoneNumber,
        string deviceId)
    {
        Token = token.Trim();
        PhoneNumber = phoneNumber.Trim();
        DeviceId = deviceId.Trim();
    }
}

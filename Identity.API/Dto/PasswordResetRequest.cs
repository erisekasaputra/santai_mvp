namespace Identity.API.Dto;

public class PasswordResetRequest
{
    public string Identity { get; set; }  
    public string OtpCode { get; set; }
    public string NewPassword { get; set; }

    public PasswordResetRequest(string identity, string otpCode, string newPassword)
    {
        Identity = identity;
        OtpCode = otpCode;
        NewPassword = newPassword;
    }
}

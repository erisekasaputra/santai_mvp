namespace Identity.API.Applications.Dto;

public class PasswordResetRequest
{
    public required string Identity { get; set; }
    public required string OtpCode { get; set; }
    public required string NewPassword { get; set; }

    public PasswordResetRequest(string identity, string otpCode, string newPassword)
    {
        Identity = identity;
        OtpCode = otpCode;
        NewPassword = newPassword;
    }
}

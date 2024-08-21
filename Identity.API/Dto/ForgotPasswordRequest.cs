using Identity.Contracts;

namespace Identity.API.Dto;

public class ForgotPasswordRequest
{
    public OtpProviderType OtpProviderType { get; set; }
    public string Identity { get; set; } = string.Empty;
}


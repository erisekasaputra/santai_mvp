using Identity.API.Enumerations;

namespace Identity.API.Domain.Entities;

public class RequestOtp
{ 
    public string Token { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public OtpRequestFor OtpRequestFor { get; set; }
}

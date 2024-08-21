using Identity.Contracts;

namespace Identity.API.Dto;

public class SendOtpRequest
{
    public Guid RequestOtpId { get; set; } 
    public string RequestOtpToken { get; set; } = string.Empty;  
    public OtpProviderType ProviderType { get; set; }
}

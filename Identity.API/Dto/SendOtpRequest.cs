using Core.Enumerations; 

namespace Identity.API.Dto;

public class SendOtpRequest
{
    public Guid OtpRequestId { get; set; } 
    public string OtpRequestToken { get; set; } = string.Empty;  
    public OtpProviderType OtpProviderType { get; set; }
}

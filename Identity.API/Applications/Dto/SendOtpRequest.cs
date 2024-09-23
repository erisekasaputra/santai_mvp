using Core.Enumerations;

namespace Identity.API.Applications.Dto;

public class SendOtpRequest
{
    public required Guid OtpRequestId { get; set; }
    public required string OtpRequestToken { get; set; } = string.Empty;
    public required OtpProviderType OtpProviderType { get; set; }
}

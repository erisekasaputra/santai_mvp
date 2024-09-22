namespace Identity.API.Applications.Dto;

public class RefreshTokenRequest
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public string DeviceId { get; set; } = string.Empty;
}

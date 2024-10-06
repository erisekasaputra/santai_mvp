using Google.Apis.Auth;
namespace Identity.API.Service.Interfaces;

public interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload?> ValidateAsync(string idToken);
}

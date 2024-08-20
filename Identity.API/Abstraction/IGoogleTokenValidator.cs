using Google.Apis.Auth; 
namespace Identity.API.Abstraction;

public interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken);
}

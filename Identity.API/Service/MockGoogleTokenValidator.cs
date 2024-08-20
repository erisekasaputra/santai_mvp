using Google.Apis.Auth;
using Identity.API.Abstraction;

namespace Identity.API.Service;

public class MockGoogleTokenValidator : IGoogleTokenValidator
{
    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
    {
        GoogleJsonWebSignature.Payload payload = new GoogleJsonWebSignature.Payload
        {
            Email = "erisekasaputra@gmail.com",
            Subject = "d1b36e8c-4e4b-4c9c-9c4a-0b2a7a8f5f18",
            Audience = "GOOGLE_CLIENT_ID_123"
        };

        return await Task.FromResult(payload);
    }
}

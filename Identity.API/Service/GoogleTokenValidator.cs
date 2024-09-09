using Core.Configurations;
using Google.Apis.Auth; 
using Identity.API.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace Identity.API.Service;

public class GoogleTokenValidator : IGoogleTokenValidator
{ 
    private readonly string _clientId; 
    public GoogleTokenValidator(IOptionsMonitor<GoogleSSOConfiguration> googleConfigs)
    {
        _clientId = googleConfigs.CurrentValue.ClientId; 
    }

    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        { 
            Audience = [_clientId]
        };    

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings); 
    } 
}

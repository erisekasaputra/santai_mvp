using Core.Configurations;
using Google.Apis.Auth; 
using Identity.API.Service.Interfaces;
using Microsoft.Extensions.Options;

namespace Identity.API.Service;

public class GoogleTokenValidator(IOptionsMonitor<GoogleSSOConfiguration> googleConfigs) : IGoogleTokenValidator
{ 
    private readonly string _clientId = googleConfigs.CurrentValue.ClientId;

    public async Task<GoogleJsonWebSignature.Payload?> ValidateAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (Exception) 
        {
            return null;
        }
    } 
}

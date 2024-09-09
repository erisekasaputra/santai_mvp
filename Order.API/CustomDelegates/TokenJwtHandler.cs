using Core.SeedWorks;
using Core.Services.Interfaces;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Order.API.CustomDelegates;

public class TokenJwtHandler : DelegatingHandler
{
    private readonly ICacheService _cacheService;
    private readonly ITokenService _tokenService;
    public TokenJwtHandler(ICacheService cacheService, ITokenService tokenService)
    {
        _cacheService = cacheService;
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string keyTokenCache = CacheKey.OrderServiceCacheKey();

        var token = await _cacheService.GetAsync<string>(keyTokenCache);

        if (string.IsNullOrWhiteSpace(token))
        {
            var claims = new List<Claim>();

            var claimIdentity = new ClaimsIdentity(claims);

            var generatedToken = _tokenService.GenerateAccessToken(claimIdentity);  

            await _cacheService.SetAsync(keyTokenCache, generatedToken, TimeSpan.FromSeconds(3600));
        }
         
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
         
        var response = await base.SendAsync(request, cancellationToken);
         
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        { 
            await _cacheService.DeleteAsync(keyTokenCache);

            var claims = new List<Claim>();

            var claimIdentity = new ClaimsIdentity(claims);

            var newGeneratedToken = _tokenService.GenerateAccessToken(claimIdentity);
             
            await _cacheService.SetAsync(keyTokenCache, newGeneratedToken, TimeSpan.FromSeconds(3600));
             
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newGeneratedToken);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}
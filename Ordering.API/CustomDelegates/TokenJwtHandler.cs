using Core.SeedWorks;
using Core.Services.Interfaces;
using System.Net.Http.Headers; 

namespace Ordering.API.CustomDelegates;

public class TokenJwtHandler : DelegatingHandler
{ 
    private readonly ITokenService _tokenService;
    public TokenJwtHandler(ICacheService cacheService, ITokenService tokenService)
    { 
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string keyTokenCache = CacheKey.OrderServiceCacheKey();

        var token = await _tokenService.GenerateAccessTokenForServiceToService(keyTokenCache);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            token = await _tokenService.GenerateAccessTokenForServiceToService(keyTokenCache, true);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}
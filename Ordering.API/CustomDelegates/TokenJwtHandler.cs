using Core.SeedWorks;
using Core.Services.Interfaces;
using System.Net.Http.Headers; 

namespace Ordering.API.CustomDelegates;

public class TokenJwtHandler : DelegatingHandler
{ 
    private readonly ITokenService _tokenService;
    private readonly IUserInfoService _userInfoService;
    public TokenJwtHandler(  
        ITokenService tokenService,
        IUserInfoService userInfoService)
    { 
        _tokenService = tokenService;
        _userInfoService = userInfoService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userInfo = _userInfoService.GetUserInfo();

        if (userInfo is null) 
        {
            throw new ArgumentNullException(nameof(userInfo));  
        }

        string keyTokenCache = CacheKey.OrderServiceCacheKey();

        var token = await _tokenService.GenerateAccessTokenForServiceToService(
            userInfo.CurrentUserType, keyTokenCache);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            token = await _tokenService.GenerateAccessTokenForServiceToService(
                userInfo.CurrentUserType, keyTokenCache, true);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}
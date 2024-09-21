using Core.Services.Interfaces;
using System.Net.Http.Headers;

namespace Core.CustomDelegates;

public class TokenJwtHandler(ITokenService tokenService) : DelegatingHandler
{
    private readonly ITokenService _tokenService = tokenService;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GenerateAccessTokenForServiceToService();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}
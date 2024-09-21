using Core.Models;
using System.Security.Claims;

namespace Core.Services.Interfaces;
public interface ITokenService
{
    string GenerateAccessToken(ClaimsIdentity claims);
    string GenerateAccessTokenForServiceToService(); 
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
}

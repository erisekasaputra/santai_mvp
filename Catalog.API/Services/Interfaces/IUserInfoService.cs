using Catalog.API.Models;

namespace Catalog.API.Services.Interfaces;

public interface IUserInfoService
{
    UserClaim? GetUserInfoAsync();
}

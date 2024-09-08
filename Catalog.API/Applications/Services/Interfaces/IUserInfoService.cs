
using Core.Models;

namespace Catalog.API.Applications.Services.Interfaces;

public interface IUserInfoService
{
    UserClaim? GetUserInfoAsync();
}

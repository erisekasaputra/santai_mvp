using Core.Models;

namespace Account.API.Applications.Services.Interfaces;

public interface IUserInfoService
{
    UserClaim? GetUserInfoAsync();
}

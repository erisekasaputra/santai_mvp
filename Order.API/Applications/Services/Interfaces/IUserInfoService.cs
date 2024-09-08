using Order.API.Applications.Models;

namespace Order.API.Applications.Services.Interfaces;

public interface IUserInfoService
{
    UserClaim? GetUserInfoAsync();
}

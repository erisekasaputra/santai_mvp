using Core.Models;

namespace Core.Services.Interfaces;

public interface IUserInfoService
{
    UserClaim? GetUserInfo();
    ServiceClaim? GetServiceInfo();
}

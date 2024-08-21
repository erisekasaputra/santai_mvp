using Account.API.Model;

namespace Account.API.Infrastructures;

public interface IUserInfoService
{
    UserClaim? GetUserInfoAsync();
}

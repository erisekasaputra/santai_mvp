using Ordering.API.Applications.Dtos.Responses;

namespace Ordering.API.Applications.Services.Interfaces;

public interface IAccountServiceAPI
{
    Task<UserInfoResponseDto?> GetUserDetail(Guid userId);
}

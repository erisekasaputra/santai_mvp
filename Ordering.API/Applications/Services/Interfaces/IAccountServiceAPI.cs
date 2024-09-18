using Ordering.API.Applications.Dtos.Responses; 

namespace Ordering.API.Applications.Services.Interfaces;

public interface IAccountServiceAPI
{
    Task<(ResultResponseDto<TDataType>?, bool isSuccess)> GetUserDetail<TDataType>(Guid userId, IEnumerable<Guid> fleetIds);
}

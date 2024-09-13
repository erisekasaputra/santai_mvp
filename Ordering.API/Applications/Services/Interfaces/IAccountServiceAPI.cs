using Ordering.API.Applications.Dtos.Responses;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Applications.Services.Interfaces;

public interface IAccountServiceAPI
{
    Task<(ResultResponseDto<TDataType>?, bool isSuccess)> GetUserDetail<TDataType>(Guid userId, IEnumerable<Guid> fleetIds);
}

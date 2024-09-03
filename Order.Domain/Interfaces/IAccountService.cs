using Order.Domain.Enumerations;

namespace Order.Domain.Interfaces;

public interface IAccountService
{
    Task<string?> GetTimeZoneByUserIdAsync(Guid userId, string userType);
}

namespace Order.API.Applications.Services.Interfaces;

public interface IAccountService
{
    Task<string?> GetTimeZoneByUserIdAsync(Guid userId, string userType);
}

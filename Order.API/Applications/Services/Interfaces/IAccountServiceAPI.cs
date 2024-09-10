namespace Order.API.Applications.Services.Interfaces;

public interface IAccountServiceAPI
{
    Task<string?> GetTimeZoneByUserIdAsync(Guid userId, string userType);
}

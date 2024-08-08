namespace Account.API.Applications.Services;

public interface IIdempotencyService
{
    Task<bool> CheckIdempotencyKeyAsync(string key);
    Task<bool> SetIdempotencyKeyAsync(string key, TimeSpan ttl);
}

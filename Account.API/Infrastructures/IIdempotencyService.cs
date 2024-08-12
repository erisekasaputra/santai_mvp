namespace Account.API.Infrastructures;

public interface IIdempotencyService
{
    Task<bool> CheckIdempotencyKeyAsync(string key);
    Task<bool> SetIdempotencyKeyAsync(string key, TimeSpan ttl);
}

namespace Identity.API.Service.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration);
    Task<bool> CheckIdempotencyKeyAsync(string key);
    Task<bool> SetIdempotencyKeyAsync(string key, TimeSpan ttl);
    Task<bool> DeleteAsync(string key);
}

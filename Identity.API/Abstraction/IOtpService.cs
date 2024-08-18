using Identity.API.Domain.Entities;

namespace Identity.API.Abstraction;

public interface IOtpService
{
    Task<(string? otp, int remainLock)> GenerateOtpAsync(string phoneNumber);
    Task<(bool statusLock, int remainLock)> CheckIsLockReleasedAsync(string phoneNumber);
    Task<Otp?> GetOtpAsync(string phoneNumber);
    Task<bool> IsOtpValidAsync(string phoneNumber, string token);
    Task<bool> RemoveOtpAsync(string phoneNumber);
}

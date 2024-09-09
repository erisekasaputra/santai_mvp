using Identity.API.Domain.Entities;
using Identity.API.Domain.Enumerations;

namespace Identity.API.Service.Interfaces;

public interface IOtpService
{
    Task<(string? otp, int remainLock)> GenerateOtpAsync(string phoneNumber);
    Task<(bool statusLock, int remainLock)> CheckIsLockReleasedAsync(string phoneNumber);
    Task<Otp?> GetOtpAsync(string phoneNumber);
    Task<bool> IsOtpValidAsync(string phoneNumber, string token);
    Task<bool> RemoveOtpAsync(string phoneNumber);
    Task<(Guid requestId, string token)> GenerateRequestOtpAsync(string phoneNumber, string? email, OtpRequestFor otpRequestFor);
    bool IsGenerateRequestOtpValidAsync(RequestOtp requestOtp, string token);
    Task<RequestOtp?> GetRequestOtpAsync(Guid requestId);
}

using Identity.API.Abstraction;
using Identity.API.Configs;
using Identity.API.Domain.Entities; 
using Identity.API.SeedWork;
using Identity.API.Utilities;
using Microsoft.Extensions.Options;
using System;

namespace Identity.API.Service;

public class OtpService : IOtpService
{
    const int treshold = 2; 
    private readonly ICacheService _cacheService;
    private readonly OtpConfig _otpConfig;
    public OtpService(ICacheService cacheService, IOptionsMonitor<OtpConfig> otpConfig)
    {
        _cacheService = cacheService;
        _otpConfig = otpConfig.CurrentValue;
    }

    public async Task<(bool, int)> CheckIsLockReleasedAsync(string phoneNumber)
    {
        var storedOtp = await GetOtpAsync(phoneNumber);

        if (storedOtp is null)
        {
            return (true, 0);
        }

        var lockTime = storedOtp.LockTimeUnix;
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (lockTime < currentTime)
        {
            return (true, 0);
        }

        int remainingTime = (int)(lockTime - currentTime);  

        return (false, remainingTime); 
    }

    public async Task<(string?, int)> GenerateOtpAsync(string phoneNumber)
    {
        int lockingTime = (_otpConfig.LockTimeSecond <= 0 ? 60 : _otpConfig.LockTimeSecond);
          
        (var isLockReleased, var remainingTime) = await CheckIsLockReleasedAsync(phoneNumber);

        if (!isLockReleased)
        {
            return (null, remainingTime);
        } 

        var otp = SecretGenerator.GenerateOtp();
        var otpObject = new Otp()
        {
            HashedPhoneNumber = phoneNumber.HashToken(),
            LockTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + lockingTime + treshold,
            HashedToken = otp.HashToken(),
        };

        await _cacheService.DeleteAsync(CacheKey.OtpCacheKey(phoneNumber.HashToken()));

        await _cacheService.SetAsync(CacheKey.OtpCacheKey(phoneNumber.HashToken()), otpObject, TimeSpan.FromHours(2));

        return (otp, lockingTime);
    }

    public async Task<Otp?> GetOtpAsync(string phoneNumber)
    {
        var otp = await _cacheService.GetAsync<Otp>(CacheKey.OtpCacheKey(phoneNumber.HashToken()));

        return otp;
    }

    public async Task<bool> IsOtpValidAsync(string phoneNumber, string token)
    {
        var otp = await GetOtpAsync(phoneNumber);

        if(otp is null)
        {
            return false;
        }

        if (otp.HashedToken == token.HashToken())
        {
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveOtpAsync(string phoneNumber)
    {
        return await _cacheService.DeleteAsync(CacheKey.OtpCacheKey(phoneNumber.HashToken()));
    }
}

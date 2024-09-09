
using Core.Configurations;
using Core.Services.Interfaces;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Enumerations;
using Identity.API.SeedWork;
using Identity.API.Service.Interfaces;
using Identity.API.Utilities;
using Microsoft.Extensions.Options;

namespace Identity.API.Service;

public class OtpService : IOtpService
{
    const int treshold = 2; 
    private readonly ICacheService _cacheService;
    private readonly OtpConfiguration _otpConfig;
    public OtpService(ICacheService cacheService, IOptionsMonitor<OtpConfiguration> otpConfig)
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

        await _cacheService.SetAsync(CacheKey.OtpCacheKey(phoneNumber.HashToken()), otpObject, TimeSpan.FromHours(1));

        return (otp, lockingTime);
    }

    public async Task<(Guid requestId, string token)> GenerateRequestOtpAsync(string phoneNumber, string? email, OtpRequestFor otpRequestFor)
    {
        var id = Guid.NewGuid();

        var secret = SecretGenerator.GenerateRandomSecret();

        var otpRequest = new RequestOtp()
        {  
            Token = secret.HashToken(),
            PhoneNumber = phoneNumber,
            Email = email,
            OtpRequestFor = otpRequestFor,
        }; 

        await _cacheService.SetAsync(CacheKey.RequestOtpCacheKey(id.ToString().HashToken()), otpRequest, TimeSpan.FromHours(1));

        return (id, secret);
    }

    public async Task<Otp?> GetOtpAsync(string phoneNumber)
    {
        var otp = await _cacheService.GetAsync<Otp>(CacheKey.OtpCacheKey(phoneNumber.HashToken()));

        return otp;
    }


    public async Task<RequestOtp?> GetRequestOtpAsync(Guid requestId)
    {
        return await _cacheService.GetAsync<RequestOtp>(CacheKey.RequestOtpCacheKey(requestId.ToString().HashToken()));
    }


    public bool IsGenerateRequestOtpValidAsync(RequestOtp requestOtp, string token)
    {  
        return (requestOtp is not null && requestOtp.Token == token.HashToken());
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

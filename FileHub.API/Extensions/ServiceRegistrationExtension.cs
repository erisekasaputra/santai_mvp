using Core.Services;
using FileHub.API.Services.Interfaces;
using FileHub.API.Services;
using Microsoft.AspNetCore.Http.Features;
using Core.Services.Interfaces;
using Core.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting; 
using Core.Policies;

namespace FileHub.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        var storageConfigs = (services.BuildServiceProvider().GetService<IOptionsMonitor<StorageConfiguration>>()?.CurrentValue
            ?? throw new Exception("Please provide value for message bus options")) ?? throw new Exception("Storage configuration has not been set");

        services.AddSingleton<ICacheService, CacheService>();
        if (storageConfigs.UseMinio)
        {
            services.AddSingleton<IStorageService, MinioStorageService>();
        }
        else
        {
            services.AddSingleton<IStorageService, AwsStorageService>();
        }

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = storageConfigs.MultipartBodyLengthLimit; // 10 MB 
        });

        return services;
    }

    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services) 
    {
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter(RateLimiterPolicy.FileUploadRateLimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 500;
                configureOptions.QueueLimit = 50;
                configureOptions.TokensPerPeriod = 500;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });

            options.AddTokenBucketLimiter(RateLimiterPolicy.FileReadRateLimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 1000;
                configureOptions.QueueLimit = 100;
                configureOptions.TokensPerPeriod = 1000;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });
        });

        return services;
    }
}

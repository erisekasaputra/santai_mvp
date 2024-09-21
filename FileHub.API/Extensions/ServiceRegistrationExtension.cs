using Core.Services;
using FileHub.API.Services.Interfaces;
using FileHub.API.Services;
using Microsoft.AspNetCore.Http.Features;
using Core.Services.Interfaces;
using Core.Configurations; 
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting; 
using Core.Policies;

namespace FileHub.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(StorageConfiguration.SectionName).Get<StorageConfiguration>()
            ?? throw new Exception();

        builder.Services.AddSingleton<ICacheService, CacheService>();
        if (options.UseMinio)
        {
            builder.Services.AddSingleton<IStorageService, MinioStorageService>();
        }
        else
        {
            builder.Services.AddSingleton<IStorageService, AwsStorageService>();
        }

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = options.MultipartBodyLengthLimit; // 10 MB 
        });

        return builder;
    }

    public static WebApplicationBuilder AddCustomRateLimiter(this WebApplicationBuilder builder) 
    {
        builder.Services.AddRateLimiter(configure =>
        {
            configure.AddTokenBucketLimiter(RateLimiterPolicy.FileUploadRateLimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 500;
                configureOptions.QueueLimit = 50;
                configureOptions.TokensPerPeriod = 500;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });

            configure.AddTokenBucketLimiter(RateLimiterPolicy.FileReadRateLimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 1000;
                configureOptions.QueueLimit = 100;
                configureOptions.TokensPerPeriod = 1000;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });
        });

        return builder;
    }
}

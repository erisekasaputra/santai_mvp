using Core.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;   

namespace Core.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddMediatorService<T>(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblyContaining<T>();
        });

        return services;
    }




    public static IServiceCollection AddRedisDatabase(this IServiceCollection services)
    {
        var cacheOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<CacheConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurations = new ConfigurationOptions
            {
                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
            };

            return ConnectionMultiplexer.Connect(configurations);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions
            {

                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
            };
        });






        services.AddOutputCache(policy =>
        {
            policy.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(cacheOptions.CurrentValue.CacheLifeTime);
        })
        .AddStackExchangeRedisOutputCache(redisOptions =>
        {
            redisOptions.ConfigurationOptions = new ConfigurationOptions
            {

                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
            };
        });



        return services;
    }


    public static IServiceCollection AddSqlDatabaseContext<T>(this IServiceCollection services) where T : DbContext
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<T>(options =>
        {
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                action.CommandTimeout(databaseOption.CurrentValue.CommandTimeout); 
            });
        });

        return services;
    }


    public static IServiceCollection AddValidation<T>(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<T>();

        return services;
    }
}

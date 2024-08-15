using Account.Domain.SeedWork;
using Account.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Account.API.Options;
using Amazon.KeyManagementService;
using Account.API.Services;
using Account.API.Infrastructures; 
using Account.API.Middleware;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;  

namespace Account.API.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddMediatorService(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblyContaining<Program>();
        });

        return services;
    }

    public static IServiceCollection AddRedisDatabase(this IServiceCollection services)
    {
        var cacheOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<InMemoryDatabaseOption>>() 
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
        }); 

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<ApplicationService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddSingleton<ICacheService, AccountCacheService>();

        return services;
    }

    public static IServiceCollection AddSqlDatabaseContext(this IServiceCollection services)
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseOption>>() 
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<AccountDbContext>((serviceProvider, options) =>
        { 
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                action.CommandTimeout(databaseOption.CurrentValue.CommandTimeOut);
                action.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
            });
        });

        return services;
    }

    public static IServiceCollection AddMassTransitContext(this IServiceCollection services)
    { 
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessageBusOption>>() 
            ?? throw new Exception("Please provide value for message bus options");

        var options = messageBusOptions.CurrentValue;
         
        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AccountDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(options.DuplicateDetectionWindows);
                o.QueryDelay = TimeSpan.FromSeconds(options.QueryDelay);
                o.QueryTimeout = TimeSpan.FromSeconds(options.QueryTimeout);
                o.QueryMessageLimit = options.QueryMessageLimit;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(options.MessageRetryInternal, 
                        TimeSpan.FromSeconds(options.MessageRetryTimespan));
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout);
                });

                configure.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static WebApplicationBuilder AddLoggingContext(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        return builder;
    }

    public static WebApplicationBuilder AddOptionConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ReferralProgramOption>(builder.Configuration.GetSection(ReferralProgramOption.SectionName)); 
        builder.Services.Configure<DatabaseOption>(builder.Configuration.GetSection(DatabaseOption.SectionName));
        builder.Services.Configure<InMemoryDatabaseOption>(builder.Configuration.GetSection(InMemoryDatabaseOption.SectionName));
        builder.Services.Configure<MessageBusOption>(builder.Configuration.GetSection(MessageBusOption.SectionName));
        builder.Services.Configure<KeyManagementServiceOption>(builder.Configuration.GetSection(KeyManagementServiceOption.SectionName));
        builder.Services.Configure<IdempotencyOptions>(builder.Configuration.GetSection(IdempotencyOptions.SectionName));
        
        return builder;
    }

    public static IServiceCollection AddDataEncryption(this IServiceCollection services, IConfiguration configuration)
    {
        // if you want to use aws as key management service, please edit aws-secret.bat inside solutions items based on your key that you got from aws. set execute bat file to the file will be started

        bool production = false;
        if (production)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions()); 
            services.AddAWSService<IAmazonKeyManagementService>(); 
            services.AddSingleton<IKeyManagementService>(sp =>
            {
                var kmsClient = sp.GetRequiredService<IAmazonKeyManagementService>();
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<KeyManagementServiceOption>>();
                return new CloudKeyManagementService(kmsClient, kmsOptions.CurrentValue.Id);
            }); 
        }
        else
        { 
            services.AddSingleton<IKeyManagementService>(sp =>
            { 
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<KeyManagementServiceOption>>();
                return new LocalKeyManagementService(kmsOptions.CurrentValue.SecretKey);
            });
        }
        services.AddSingleton<IHashService, HashService>();

        return services;
    }

    public static IServiceCollection AddJsonEnumConverterBehavior(this IServiceCollection builder)
    {
        builder.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters
                .Add(new JsonStringEnumConverter(
                    namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase,
                    allowIntegerValues: true));
        }); 

        return builder;
    }
}

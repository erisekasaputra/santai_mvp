using Account.API.Applications.Services; 
using Account.Domain.SeedWork;
using Account.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis; 
using MassTransit;
using Microsoft.EntityFrameworkCore; 
using Account.API.Options;

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
        var cacheOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<InMemoryDatabaseOption>>() ?? throw new Exception("Please provide value for database option");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurations = new ConfigurationOptions
            {
                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan.FromSeconds(3).TotalMilliseconds)
            };

            return ConnectionMultiplexer.Connect(configurations);
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
        services.AddScoped<AppService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        return services;
    }

    public static IServiceCollection AddSqlDatabaseContext(this IServiceCollection services)
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseOption>>() ?? throw new Exception("Please provide value for database option");

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
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessageBusOption>>() ?? throw new Exception("Please provide value for message bus options");

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AccountDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.QueryTimeout = TimeSpan.FromSeconds(30);
                o.QueryMessageLimit = 100;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messageBusOptions.CurrentValue.Host ?? string.Empty, host =>
                {
                    host.Username(messageBusOptions.CurrentValue.Username ?? string.Empty);
                    host.Password(messageBusOptions.CurrentValue.Password ?? string.Empty);
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(3, TimeSpan.FromSeconds(2));
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(5);
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
        
        return builder;
    }
}

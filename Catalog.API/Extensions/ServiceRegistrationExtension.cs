
using Catalog.API.Applications.Services;
using Catalog.API.Applications.Services.Interfaces; 
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Helpers;
using Core.Configurations;
using Core.Enumerations;
using Core.SeedWorks;
using FluentValidation;
using FluentValidation.AspNetCore; 
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

namespace Catalog.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddMediatorService(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblies(typeof(ICatalogAPIMarkerInterface).Assembly); 
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




        services.AddOutputCache(options =>
        {
            options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(cacheOptions.CurrentValue.CacheLifeTime);
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

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<ICatalogAPIMarkerInterface>();

        return services;
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        //services.AddTransient<TokenJwtHandler>();
        //services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserInfoService, UserService>();
        services.AddSingleton<ICacheService, CatalogCacheService>();

        services.AddScoped<ApplicationService>(); 
        services.AddScoped<MetaTableHelper>();

        return services;
    }
     
    public static IServiceCollection AddSqlDatabaseContext(this IServiceCollection services)
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                action.CommandTimeout(databaseOption.CurrentValue.CommandTimeout);
            });
        });

        return services;
    }

    public static IServiceCollection AddMassTransitContext(this IServiceCollection services)
    {
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        var options = messageBusOptions.CurrentValue;

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.QueryTimeout = TimeSpan.FromSeconds(30);
                o.QueryMessageLimit = 100;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            //x.AddConsumersFromNamespaceContaining<ICatalogAPIMarkerInterface>();

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messageBusOptions.CurrentValue.Host
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)), host =>
                {
                    host.Username(messageBusOptions.CurrentValue.Username
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)));
                    host.Password(messageBusOptions.CurrentValue.Password
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)));
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
        builder.Services.Configure<AccountServiceConfiguration>(builder.Configuration.GetSection(AccountServiceConfiguration.SectionName));
        builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection(DatabaseConfiguration.SectionName));
        builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection(CacheConfiguration.SectionName));
        builder.Services.Configure<MessagingConfiguration>(builder.Configuration.GetSection(MessagingConfiguration.SectionName));
        builder.Services.Configure<EncryptionConfiguration>(builder.Configuration.GetSection(EncryptionConfiguration.SectionName));
        builder.Services.Configure<IdempotencyConfiguration>(builder.Configuration.GetSection(IdempotencyConfiguration.SectionName));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));

        return builder;
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

        builder.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return builder;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var jwtOption = services.BuildServiceProvider().GetService<IOptionsMonitor<JwtConfiguration>>()
           ?? throw new Exception("Please provide value for message bus options");

        var jwt = jwtOption.CurrentValue;

        services.AddAuthentication(authOption =>
        {
            authOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwt?.SecretKey ?? throw new Exception("Secret key for jwt can not be empty"));

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.FromSeconds(0),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt?.Issuer ?? throw new Exception("Issuer can not be null"),
                ValidAudience = jwt?.Audience ?? throw new Exception("Audience can not be null"),
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyName.RegularUserPolicy, policy =>
            {
                policy.RequireRole(UserType.RegularUser.ToString());
            });

            options.AddPolicy(PolicyName.BusinessUserPolicy, policy =>
            {
                policy.RequireRole(UserType.BusinessUser.ToString());
            });

            options.AddPolicy(PolicyName.StaffUserPolicy, policy =>
            {
                policy.RequireRole(UserType.StaffUser.ToString());
            });

            options.AddPolicy(PolicyName.MechanicUserPolicy, policy =>
            {
                policy.RequireRole(UserType.MechanicUser.ToString());
            });

            options.AddPolicy(PolicyName.AdministratorPolicy, policy =>
            {
                policy.RequireRole(UserType.Administrator.ToString());
            });
        });

        return services;
    }
}

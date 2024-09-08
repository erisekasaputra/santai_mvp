using Account.API.Consumers;
using Account.API.Infrastructures;
using Account.API.Middleware;
using Account.API.Options;
using Account.API.SeedWork;
using Account.API.Services;
using Account.Domain.SeedWork;
using Account.Infrastructure;
using Amazon.KeyManagementService;
using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Contracts.Enumerations;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

namespace Account.API.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddMediatorService(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblyContaining<IAccountAPIMarkerInterface>();
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
        services.AddValidatorsFromAssemblyContaining<IAccountAPIMarkerInterface>();

        return services;
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IUserInfoService, UserService>();
        services.AddScoped<ApplicationService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddSingleton<ICacheService, AccountCacheService>();

        return services;
    }

    public static IServiceCollection AddSqlDatabaseContext(this IServiceCollection services)
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseOption>>() 
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<AccountDbContext>(options =>
        { 
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                action.CommandTimeout(databaseOption.CurrentValue.CommandTimeOut);
                action.MigrationsAssembly("Account.Infrastructure");
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

            x.AddConsumer<IdentityEmailAssignedToAUserIntegrationEventConsumer>();
            x.AddConsumer<IdentityPhoneNumberConfirmedIntegrationEventConsumer>();
            x.AddConsumer<PhoneNumberDuplicateIntegrationEventConsumer>();

            x.UsingRabbitMq((context, configure) =>
            {  
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(options.MessageRetryInterval, 
                        TimeSpan.FromSeconds(options.MessageRetryTimespan));
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout);
                });

                configure.ConfigureEndpoints(context);  

                var consumers = new (string QueueName, Type ConsumerType)[]
                {
                    ("identity-email-assigned-to-a-user-integration-event-queue", typeof(IdentityEmailAssignedToAUserIntegrationEventConsumer)),
                    ("identity-phone-number-confirmed-integration-event-queue", typeof(IdentityPhoneNumberConfirmedIntegrationEventConsumer)),
                    ("phone-number-duplicate-integration-event-queue", typeof(PhoneNumberDuplicateIntegrationEventConsumer))
                };

                foreach (var (queueName, consumerType) in consumers)
                {
                    configure.ReceiveEndpoint(queueName, receiveBuilder =>
                    {
                        ConfigureEndPoint(receiveBuilder, queueName, consumerType);
                    });
                }

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                { 
                    receiveBuilder.UseMessageRetry(retry =>
                    {
                        retry.Interval(options.MessageRetryInterval, TimeSpan.FromSeconds(options.MessageRetryTimespan));
                    });

                    receiveBuilder.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInternval));
                    });

                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
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
        builder.Services.Configure<JwtOption>(builder.Configuration.GetSection(JwtOption.SectionName));
        
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

        builder.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return builder;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var jwtOption = services.BuildServiceProvider().GetService<IOptionsMonitor<JwtOption>>()
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

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens; 
using Order.API.Applications.Services;
using Order.Domain.SeedWork;
using Order.Infrastructure;  
using Amazon.KeyManagementService;
using Order.API.Applications.Services.Interfaces;
using Order.API.CustomDelegates;
using Core.Configurations;
using Core.SeedWorks;
using Core.Enumerations;

namespace Order.API.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddMediatorService(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblyContaining<IOrderApiMarkerInterface>();
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
        services.AddValidatorsFromAssemblyContaining<IOrderApiMarkerInterface>();

        return services;
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddTransient<TokenJwtHandler>(); 
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddScoped<IUserInfoService, UserService>(); 
        services.AddSingleton<ICacheService, OrderCacheService>();

        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection service)
    {
        var clientConfig = service.BuildServiceProvider().GetService<IOptionsMonitor<AccountServiceConfiguration>>()
            ?? throw new Exception("Please provide value for database option");



        service.AddHttpClient<IAccountService, AccountService>(client =>
        {
            client.BaseAddress = new Uri(clientConfig?.CurrentValue.Host ?? throw new Exception("Account service client host is not set"));
        })
        .AddHttpMessageHandler<TokenJwtHandler>(); 

        return service;
    }

    public static IServiceCollection AddSqlDatabaseContext(this IServiceCollection services)
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<OrderDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                action.EnableRetryOnFailure();
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
            x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(options.DuplicateDetectionWindows);
                o.QueryDelay = TimeSpan.FromSeconds(options.QueryDelay);
                o.QueryTimeout = TimeSpan.FromSeconds(options.QueryTimeout);
                o.QueryMessageLimit = options.QueryMessageLimit;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.AddConsumersFromNamespaceContaining<IOrderApiMarkerInterface>();

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
                    //("identity-email-assigned-to-a-user-integration-event-queue", typeof(IdentityEmailAssignedToAUserIntegrationEventConsumer))
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
                        redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval));
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
        builder.Services.Configure<AccountServiceConfiguration>(builder.Configuration.GetSection(AccountServiceConfiguration.SectionName));
        builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection(DatabaseConfiguration.SectionName));
        builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection(CacheConfiguration.SectionName));
        builder.Services.Configure<MessagingConfiguration>(builder.Configuration.GetSection(MessagingConfiguration.SectionName));
        builder.Services.Configure<EncryptionConfiguration>(builder.Configuration.GetSection(EncryptionConfiguration.SectionName));
        builder.Services.Configure<IdempotencyConfiguration>(builder.Configuration.GetSection(IdempotencyConfiguration.SectionName));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));

        return builder;
    }

    public static IServiceCollection AddDataEncryption(this IServiceCollection services, IConfiguration configuration)
    {
        bool production = false;
        if (production)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonKeyManagementService>();
            services.AddSingleton<IKeyManagementService>(sp =>
            {
                var kmsClient = sp.GetRequiredService<IAmazonKeyManagementService>();
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<EncryptionConfiguration>>();
                return new CloudKeyManagementService(kmsClient, kmsOptions.CurrentValue.Id);
            });
        }
        else
        {
            services.AddSingleton<IKeyManagementService>(sp =>
            {
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<EncryptionConfiguration>>();
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

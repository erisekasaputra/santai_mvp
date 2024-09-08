using Account.API.Applications.Consumers;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces; 
using Account.Domain.SeedWork;
using Account.Infrastructure;
using Amazon.KeyManagementService;
using Core.Configurations;
using Core.Enumerations;
using Core.SeedWorks; 
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens; 
using System.Text;
using System.Text.Json.Serialization;

namespace Account.API.Extensions;

public static class ServiceRegistrationExtension
{   
 

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IUserInfoService, UserService>();
        services.AddScoped<ApplicationService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddSingleton<ICacheService, AccountCacheService>();

        return services;
    } 

    public static IServiceCollection AddMassTransitContext(this IServiceCollection services)
    { 
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>() 
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
        builder.Services.Configure<ReferralProgramConfiguration>(builder.Configuration.GetSection(ReferralProgramConfiguration.SectionName)); 
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

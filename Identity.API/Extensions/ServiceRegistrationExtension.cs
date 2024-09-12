using Core.Policies;
using Core.Utilities;
using Identity.API.Middleware;
using Identity.API.Service.Interfaces;
using Identity.API.Service;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Core.Services;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.Configurations;
using Microsoft.Extensions.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Identity.API.Consumers;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure; 
namespace Identity.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter(RateLimiterPolicy.AuthenticationRateRimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 500;
                configureOptions.QueueLimit = 50;
                configureOptions.TokensPerPeriod = 500;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureOtpService(this IServiceCollection services) 
    {
        var otpOption = services.BuildServiceProvider().GetService<IOptionsMonitor<OtpConfiguration>>()?.CurrentValue
            ?? throw new Exception("Please provide value for otp configuration");

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            ArgumentNullException.ThrowIfNull(otpOption);

            options.TokenLifespan = TimeSpan.FromSeconds(otpOption.LockTimeSecond);
        });

        return services;
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services) 
    {
        services.AddTransient<GlobalExceptionMiddleware>();
        services.AddSingleton<ActionMethodUtility>(); 
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>(); 
        services.AddSingleton<IGoogleTokenValidator, MockGoogleTokenValidator>(); 
        services.AddSingleton<IOtpService, OtpService>();
        services.AddSingleton<ICacheService, CacheService>(); 

        return services;
    }  

    public static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedPhoneNumber = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;

            options.User.RequireUniqueEmail = false;
             
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 0;
            options.Password.RequiredUniqueChars = 0;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders()
        .AddClaimsPrincipalFactory<ApplicationClaims>();

        return services;
    }


    public static IServiceCollection AddMasstransitContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var messagingOption = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()?.CurrentValue
           ?? throw new Exception("Please provide value for messaging configuration");

        services.AddMassTransit(x =>
        {
            if (messagingOption is null)
            {
                throw new Exception("Messaging configuration has not been set");
            }

            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("identity-service-business-user-created-integration-event-queue", typeof(BusinessUserCreatedIntegrationEventConsumer))
            };

            foreach ((_, Type consumerType) in consumers)
            {
                x.AddConsumer(consumerType);
            }

            x.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(messagingOption.DuplicateDetectionWindows);
                o.QueryDelay = TimeSpan.FromSeconds(messagingOption.QueryDelay);
                o.QueryTimeout = TimeSpan.FromSeconds(messagingOption.QueryTimeout);
                o.QueryMessageLimit = messagingOption.QueryMessageLimit;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

           

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messagingOption.Host, host =>
                {
                    host.Username(messagingOption.Username ?? "user");
                    host.Password(messagingOption.Password ?? "user");
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(
                        messagingOption.MessageRetryInterval, 
                        messagingOption.MessageRetryTimespan);
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(messagingOption.MessageTimeout);
                });

                configure.ConfigureEndpoints(context); 

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
                        retry.Interval(messagingOption.MessageRetryInterval, TimeSpan.FromSeconds(messagingOption.MessageRetryTimespan));
                    });

                    receiveBuilder.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Intervals(TimeSpan.FromSeconds(messagingOption.DelayedRedeliveryInterval));
                    });

                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return services;

    }
}

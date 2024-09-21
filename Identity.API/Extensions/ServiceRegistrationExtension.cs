using Core.Policies;
using Core.Utilities; 
using Identity.API.Service.Interfaces;
using Identity.API.Service;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Core.Services;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.Configurations; 
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Identity.API.Consumers;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Core.Middlewares;
namespace Identity.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddCustomRateLimiter(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(configure =>
        {
            configure.AddTokenBucketLimiter(RateLimiterPolicy.AuthenticationRateRimiterPolicy, configureOptions =>
            {
                configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configureOptions.TokenLimit = 500;
                configureOptions.QueueLimit = 50;
                configureOptions.TokensPerPeriod = 500;
                configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            });
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureOtpService(this WebApplicationBuilder builder) 
    {
        var options = builder.Configuration.GetSection(OtpConfiguration.SectionName).Get<OtpConfiguration>() ?? throw new Exception("Please provide value for otp configuration"); 
        builder.Services.Configure<DataProtectionTokenProviderOptions>(configure =>
        {
            ArgumentNullException.ThrowIfNull(options);
            configure.TokenLifespan = TimeSpan.FromSeconds(options.LockTimeSecond);
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder) 
    {
        builder.Services.AddTransient<GlobalExceptionMiddleware>();
        builder.Services.AddSingleton<ActionMethodUtility>(); 
        builder.Services.AddScoped<ITokenService, JwtTokenService>();
        builder.Services.AddScoped<IJwtTokenValidator, JwtTokenValidator>(); 
        builder.Services.AddScoped<IGoogleTokenValidator, MockGoogleTokenValidator>(); 
        builder.Services.AddSingleton<IOtpService, OtpService>();
        builder.Services.AddSingleton<ICacheService, CacheService>(); 
        builder.Services.AddScoped<IUserInfoService, UserInfoService>();
        builder.Services.AddScoped<ITokenCacheService, TokenCacheService>();

        return builder;
    }  

    public static WebApplicationBuilder AddIdentityService(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(configure =>
        {
            configure.SignIn.RequireConfirmedPhoneNumber = true;
            configure.SignIn.RequireConfirmedEmail = false;
            configure.SignIn.RequireConfirmedAccount = false;

            configure.User.RequireUniqueEmail = false;

            configure.Password.RequireDigit = false;
            configure.Password.RequireLowercase = false;
            configure.Password.RequireNonAlphanumeric = false;
            configure.Password.RequireUppercase = false;
            configure.Password.RequiredLength = 0;
            configure.Password.RequiredUniqueChars = 0;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders()
        .AddClaimsPrincipalFactory<ApplicationClaims>();

        return builder;
    }


    public static WebApplicationBuilder AddMasstransitContext<TDbContext>(this WebApplicationBuilder builder) where TDbContext : DbContext
    {
        var options = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>()
           ?? throw new Exception();

        builder.Services.AddMassTransit(x =>
        { 
            x.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(options.DuplicateDetectionWindows);
                o.QueryDelay = TimeSpan.FromSeconds(options.QueryDelay);
                o.QueryTimeout = TimeSpan.FromSeconds(options.QueryTimeout);
                o.QueryMessageLimit = options.QueryMessageLimit;
                o.UseSqlServer();
                o.UseBusOutbox();
            }); 

            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("identity-service-business-user-created-integration-event-queue", typeof(BusinessUserCreatedIntegrationEventConsumer)),
                ("identity-service-business-user-deleted-integration-event-queue", typeof(BusinessUserDeletedIntegrationEventConsumer)),
                ("identity-service-mechanic-user-created-integration-event-queue", typeof(MechanicUserCreatedIntegrationEventConsumer)),
                ("identity-service-mechanic-user-deleted-integration-event-queue", typeof(MechanicUserDeletedIntegrationEventConsumer)),
                ("identity-service-regular-user-created-integration-event-queue", typeof(RegularUserCreatedIntegrationEventConsumer)),
                ("identity-service-regular-user-deleted-integration-event-queue", typeof(RegularUserDeletedIntegrationEventConsumer)),
                ("identity-service-staff-user-created-integration-event-queue", typeof(StaffUserCreatedIntegrationEventConsumer)),
                ("identity-service-staff-user-deleted-integration-event-queue", typeof(StaffUserDeletedIntegrationEventConsumer)),
            };

            x.AddConsumersFromNamespaceContaining<IIdentityMarkerInterface>();
            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseTimeout(timeoutCfg => timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout));

                foreach (var (queueName, consumerType) in consumers)
                    configure.ReceiveEndpoint(queueName, receiveBuilder => ConfigureEndPoint(receiveBuilder, queueName, consumerType));

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                {
                    receiveBuilder.ConfigureConsumer(context, consumerType);
                    receiveBuilder.UseMessageRetry(retry => retry.Interval(options.MessageRetryInterval, TimeSpan.FromSeconds(options.MessageRetryTimespan)));
                    receiveBuilder.UseDelayedRedelivery(redelivery => redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval)));
                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return builder;

    }
}

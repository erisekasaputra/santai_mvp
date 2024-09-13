using Account.API.Applications.Consumers;
using Account.API.Applications.Services; 
using Account.Domain.SeedWork;
using Account.Infrastructure; 
using Core.Configurations; 
using Core.Services;
using Core.Services.Interfaces;
using Hangfire;
using MassTransit; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options; 

namespace Account.API.Extensions;

public static class ServiceRegistrationExtension
{    
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<ApplicationService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<OrderJobService>(); 
        return services;
    } 

    public static IServiceCollection AddMassTransitContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    { 
        var messagingOption = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>() 
            ?? throw new Exception("Please provide value for message bus options");

        var options = messagingOption.CurrentValue;
         
        services.AddMassTransit(x =>
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
                    ("account-service-identity-email-assigned-to-a-user-integration-event-queue", typeof(IdentityEmailAssignedToAUserIntegrationEventConsumer)),
                    ("account-service-identity-phone-number-confirmed-integration-event-queue", typeof(IdentityPhoneNumberConfirmedIntegrationEventConsumer)),
                    ("account-service-phone-number-duplicate-integration-event-queue", typeof(PhoneNumberDuplicateIntegrationEventConsumer))
                };

            foreach((_, Type consumerType) in consumers)
            {
                x.AddConsumer(consumerType); 
            }


            x.UsingRabbitMq((context, configure) =>
            {  
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(
                        options.MessageRetryInterval,
                        options.MessageRetryTimespan);
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout);
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

    public static IServiceCollection AddHangfireContext(this IServiceCollection services)
    {
        var messagingOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
         ?? throw new Exception("Please provide value for message bus options");
          
        services.AddHangfire(config =>
            config.UseSqlServerStorage(messagingOption.CurrentValue.ConnectionString));
         
        services.AddHangfireServer(); 

        return services;
    }
}

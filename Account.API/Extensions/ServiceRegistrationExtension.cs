using Account.API.Applications.Consumers;
using Account.API.Applications.Services; 
using Account.Domain.SeedWork;
using Account.Infrastructure; 
using Core.Configurations; 
using Core.Services;
using Core.Services.Interfaces;
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
}

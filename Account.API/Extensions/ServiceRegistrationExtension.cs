using Account.API.Applications.Consumers;
using Account.API.Applications.Services;
using Account.API.Applications.Services.Interfaces;
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
        services.AddScoped<ICacheService, CacheService>(); 
        services.AddScoped<IMechanicCache, MechanicCache>();
        services.AddHostedService<OrderWaitingMechanicAssignJob>();
        services.AddHostedService<OrderWaitingMechanicConfirmExpiryJob>();
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
                ("account-service-phone-number-duplicate-integration-event-queue", typeof(PhoneNumberDuplicateIntegrationEventConsumer)),
                ("account-service-order-finding-mechanic-integration-event-queue", typeof(OrderFindingMechanicIntegrationEventConsumer)),
                ("account-service-order-cancelled-by-mechanic-integration-event-queue", typeof(OrderCancelledByMechanicIntegrationEventConsumer)),
                ("account-service-order-cancelled-by-buyer-integration-event-queue", typeof(OrderCancelledByUserIntegrationEventConsumer)) 
            };

            //foreach ((_, Type consumerType) in consumers) x.AddConsumer(consumerType); 
            x.AddConsumersFromNamespaceContaining<IAccountAPIMarkerInterface>();
            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseTimeout(timeoutCfg => timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout));
                //configure.ConfigureEndpoints(context);

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

        return services;
    }  
}

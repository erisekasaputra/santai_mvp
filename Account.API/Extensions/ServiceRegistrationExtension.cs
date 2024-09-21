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
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserInfoService, UserInfoService>();
        builder.Services.AddScoped<ApplicationService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        builder.Services.AddScoped<ICacheService, CacheService>(); 
        builder.Services.AddScoped<IMechanicCache, MechanicCache>();
        builder.Services.AddHostedService<OrderWaitingMechanicAssignJob>();
        builder.Services.AddHostedService<OrderWaitingMechanicConfirmExpiryJob>();
        return builder;
    }  

    public static WebApplicationBuilder AddMassTransitContext<TDbContext>(this WebApplicationBuilder builder) where TDbContext : DbContext
    { 
        var options = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<IOptionsMonitor<MessagingConfiguration>>()?.CurrentValue
            ?? throw new Exception("Please provide value for message bus options"); 

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
                ("account-service-identity-email-assigned-to-a-user-integration-event-queue", typeof(IdentityEmailAssignedToAUserIntegrationEventConsumer)),
                ("account-service-identity-phone-number-confirmed-integration-event-queue", typeof(IdentityPhoneNumberConfirmedIntegrationEventConsumer)),
                ("account-service-phone-number-duplicate-integration-event-queue", typeof(PhoneNumberDuplicateIntegrationEventConsumer)),
                ("account-service-order-finding-mechanic-integration-event-queue", typeof(OrderFindingMechanicIntegrationEventConsumer)),
                ("account-service-order-cancelled-by-mechanic-integration-event-queue", typeof(OrderCancelledByMechanicIntegrationEventConsumer)),
                ("account-service-order-cancelled-by-buyer-integration-event-queue", typeof(OrderCancelledByUserIntegrationEventConsumer)), 
                ("account-service-order-service-incompleted-integration-event-queue", typeof(ServiceIncompletedIntegrationEventConsumer)), 
                ("account-service-order-service-completed-integration-event-queue", typeof(ServiceCompletedIntegrationEventConsumer))
            };
             
            x.AddConsumersFromNamespaceContaining<IAccountAPIMarkerInterface>();
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

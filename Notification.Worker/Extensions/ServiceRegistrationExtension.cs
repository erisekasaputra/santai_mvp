using Core.Configurations; 
using Core.Services;
using Core.Services.Interfaces;
using MassTransit; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notification.Worker.Consumers;
using StackExchange.Redis;
using System.CodeDom;

namespace Notification.Worker.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddMassTransitContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    { 
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options"); 
        var options = messageBusOptions.CurrentValue;

        var isAmazonSqs = false;
        if (isAmazonSqs)
        { 
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
                    ("notification-service-account-mechanic-order-accepted-integration-event-queue",typeof(AccountMechanicOrderAcceptedIntegrationEventConsumer)),
                    ("notification-service-business-license-accepted-integration-event-queue",typeof(BusinessLicenseAcceptedIntegrationEventConsumer)),
                    ("notification-service-business-license-rejected-integration-event-queue",typeof(BusinessLicenseRejectedIntegrationEventConsumer)),
                    ("notification-service-business-user-created-integration-event-queue",typeof(BusinessUserCreatedIntegrationEventConsumer)),
                    ("notification-service-business-user-deleted-integration-event-queue",typeof(BusinessUserDeletedIntegrationEventConsumer)),
                    ("notification-service-item-price-set-integration-event-queue",typeof(ItemPriceSetIntegrationEventConsumer)),
                    ("notification-service-mechanic-auto-selected-integration-event-queue",typeof(MechanicAutoSelectedIntegrationEventConsumer)),
                    ("notification-service-mechanic-dispatched-integration-event-queue",typeof(MechanicDispatchedIntegrationEventConsumer)),
                    ("notification-service-order-cancelled-by-buyer-integration-event-queue",typeof(OrderCancelledByBuyerIntegrationEventConsumer)),
                    ("notification-service-order-cancelled-by-mechanic-integration-event-queue",typeof(OrderCancelledByMechanicIntegrationEventConsumer)),
                    ("notification-service-order-created-integration-event-queue",typeof(OrderCreatedIntegrationEventConsumer)),
                    ("notification-service-order-finding-mechanic-integration-event-queue",typeof(OrderFindingMechanicIntegrationEventConsumer)),
                    ("notification-service-order-mechanic-arrived-integration-event-queue",typeof(OrderMechanicArrivedIntegrationEventConsumer)),
                    ("notification-service-order-payment-paid-integration-event-queue",typeof(OrderPaymentPaidIntegrationEventConsumer)),
                    ("notification-service-order-rated-integration-queue",typeof(OrderRatedIntegrationEventConsumer)),
                    ("notification-service-order-rejected-by-mechanic-integration-queue",typeof(OrderRejectedByMechanicIntegrationEventConsumer)),
                    ("notification-service-otp-requested-integration-queue",typeof(OtpRequestedIntegrationEventConsumer)),
                    ("notification-service-refund-paid-integration-queue",typeof(RefundPaidIntegrationEventConsumer)),
                    ("notification-service-service-completed-integration-queue",typeof(ServiceCompletedIntegrationEventConsumer)),
                    ("notification-service-service-incompleted-integration-queue",typeof(ServiceIncompletedIntegrationEventConsumer)),
                    ("notification-service-service-processed-integration-queue",typeof(ServiceProcessedIntegrationEventConsumer))
                };

                //foreach ((_, Type consumerType) in consumers) x.AddConsumer(consumerType);

                x.AddConsumersFromNamespaceContaining<INotificationWorkerMarkerInterface>(); 
                x.UsingAmazonSqs((context, configure) =>
                {
                    configure.Host(options.Host ?? string.Empty, host =>
                    {
                        host.AccessKey(options.Username ?? string.Empty);
                        host.SecretKey(options.Password ?? string.Empty);
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
        }
        else
        {
            services.AddMassTransit(x =>
            { 
                var consumers = new (string QueueName, Type ConsumerType)[]
                {
                    ("notification-service-account-mechanic-order-accepted-integration-event-queue",typeof(AccountMechanicOrderAcceptedIntegrationEventConsumer)),
                    ("notification-service-business-license-accepted-integration-event-queue",typeof(BusinessLicenseAcceptedIntegrationEventConsumer)),
                    ("notification-service-business-license-rejected-integration-event-queue",typeof(BusinessLicenseRejectedIntegrationEventConsumer)),
                    ("notification-service-business-user-created-integration-event-queue",typeof(BusinessUserCreatedIntegrationEventConsumer)),
                    ("notification-service-business-user-deleted-integration-event-queue",typeof(BusinessUserDeletedIntegrationEventConsumer)),
                    ("notification-service-item-price-set-integration-event-queue",typeof(ItemPriceSetIntegrationEventConsumer)),
                    ("notification-service-mechanic-auto-selected-integration-event-queue",typeof(MechanicAutoSelectedIntegrationEventConsumer)),
                    ("notification-service-mechanic-dispatched-integration-event-queue",typeof(MechanicDispatchedIntegrationEventConsumer)),
                    ("notification-service-order-cancelled-by-buyer-integration-event-queue",typeof(OrderCancelledByBuyerIntegrationEventConsumer)),
                    ("notification-service-order-cancelled-by-mechanic-integration-event-queue",typeof(OrderCancelledByMechanicIntegrationEventConsumer)),
                    ("notification-service-order-created-integration-event-queue",typeof(OrderCreatedIntegrationEventConsumer)),
                    ("notification-service-order-finding-mechanic-integration-event-queue",typeof(OrderFindingMechanicIntegrationEventConsumer)),
                    ("notification-service-order-mechanic-arrived-integration-event-queue",typeof(OrderMechanicArrivedIntegrationEventConsumer)),
                    ("notification-service-order-payment-paid-integration-event-queue",typeof(OrderPaymentPaidIntegrationEventConsumer)),
                    ("notification-service-order-rated-integration-queue",typeof(OrderRatedIntegrationEventConsumer)),
                    ("notification-service-order-rejected-by-mechanic-integration-queue",typeof(OrderRejectedByMechanicIntegrationEventConsumer)),
                    ("notification-service-otp-requested-integration-queue",typeof(OtpRequestedIntegrationEventConsumer)),
                    ("notification-service-refund-paid-integration-queue",typeof(RefundPaidIntegrationEventConsumer)),
                    ("notification-service-service-completed-integration-queue",typeof(ServiceCompletedIntegrationEventConsumer)),
                    ("notification-service-service-incompleted-integration-queue",typeof(ServiceIncompletedIntegrationEventConsumer)),
                    ("notification-service-service-processed-integration-queue",typeof(ServiceProcessedIntegrationEventConsumer))
                };


                //foreach ((_, Type consumerType) in consumers) x.AddConsumer(consumerType); 
                x.AddConsumersFromNamespaceContaining<INotificationWorkerMarkerInterface>();
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
        } 
        return services;  
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        var redisOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<CacheConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        services.AddSignalR().AddStackExchangeRedis(redisOptions.CurrentValue.Host, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("NotificationWorker");
        });

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}

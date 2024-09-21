using Core.Configurations; 
using Core.Services;
using Core.Services.Interfaces;
using MassTransit; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notification.Worker.Consumers;
using StackExchange.Redis; 

namespace Notification.Worker.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddMassTransitContext<TDbContext>(this WebApplicationBuilder builder) where TDbContext : DbContext
    { 
        var options = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>() ?? throw new Exception();  

        var isAmazonSqs = false;
        if (isAmazonSqs)
        {
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
            builder.Services.AddMassTransit(x =>
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
                 
                x.AddConsumersFromNamespaceContaining<INotificationWorkerMarkerInterface>();
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
        } 
        return builder;  
    }

    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() ?? throw new Exception();  
        builder.Services.AddSignalR().AddStackExchangeRedis(options.Host, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("NotificationWorker");
        }); 
        builder.Services.AddSingleton<ICacheService, CacheService>(); 
        return builder;
    }
}

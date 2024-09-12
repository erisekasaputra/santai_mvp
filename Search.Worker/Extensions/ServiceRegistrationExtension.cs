using Core.Configurations;
using MassTransit;
using Microsoft.Extensions.Options;
using Search.Worker.Consumers;
using Search.Worker.Domain.Repositories;
using Search.Worker.Infrastructure;
using Search.Worker.Infrastructure.Repositories;

namespace Search.Worker.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    { 
        services.AddScoped<ElasticsearchContext>();
        services.AddScoped<IItemRepository, ItemRepository>();
        return services;
    }

    public static IServiceCollection AddMasstransitContext(this IServiceCollection services)
    {
        var messagingConfiguration = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        var messaging = messagingConfiguration.CurrentValue;

        services.AddMassTransit(x =>
        {
            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("search-worker-service-item-created-integration-event-queue", typeof(ItemCreatedIntegrationEventConsumer)),
                ("search-worker-service-item-updated-integration-event-queue", typeof(ItemUpdatedIntegrationEventConsumer)),
                ("search-worker-service-item-deleted-integration-event-queue", typeof(ItemDeletedIntegrationEventConsumer)),
                ("search-worker-service-item-undeleted-integration-event-queue", typeof(ItemUndeletedIntegrationEventConsumer)),
                ("search-worker-service-item-activated-integration-event-queue", typeof(ItemActivatedIntegrationEventConsumer)),
                ("search-worker-service-item-inactivated-integration-event-queue", typeof(ItemInactivatedIntegrationEventConsumer)),
                ("search-worker-service-item-price-set-integration-event-queue", typeof(ItemPriceSetIntegrationEventConsumer)),
                ("search-worker-service-item-sold-added-integration-event-queue", typeof(ItemSoldAddedIntegrationEventConsumer)),
                ("search-worker-service-item-sold-reduced-integration-event-queue", typeof(ItemSoldReducedIntegrationEventConsumer)),
                ("search-worker-service-item-sold-set-integration-event-queue", typeof(ItemSoldSetIntegrationEventConsumer)),
                ("search-worker-service-item-stock-added-integration-event-queue", typeof(ItemStockAddedIntegrationEventConsumer)),
                ("search-worker-service-item-stock-reduced-integration-event-queue", typeof(ItemStockReducedIntegrationEventConsumer)),
                ("search-worker-service-item-stock-set-integration-event-queue", typeof(ItemStockSetIntegrationEventConsumer)),
                ("search-worker-service-brand-deleted-integration-event-queue", typeof(BrandDeletedIntegrationEventConsumer)),
                ("search-worker-service-brand-updated-integration-event-queue", typeof(BrandUpdatedIntegrationEventConsumer)),
                ("search-worker-service-category-deleted-integration-event-queue", typeof(CategoryDeletedIntegrationEventConsumer)),
                ("search-worker-service-category-updated-integration-event-queue", typeof(CategoryUpdatedIntegrationEventConsumer))
            };

            foreach ((_, Type consumerType) in consumers)
            {
                x.AddConsumer(consumerType);
            }

            x.UsingRabbitMq((context, config) =>
            {
                config.Host(messaging.Host ?? "localhost", host =>
                {
                    host.Username(messaging.Username ?? "user");
                    host.Password(messaging.Password ?? "user");
                });

                config.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(
                        messaging.MessageRetryInterval,
                        messaging.MessageRetryTimespan);
                });

                config.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(10);
                });  

                config.ConfigureEndpoints(context);

                foreach (var (queueName, consumerType) in consumers)
                {
                    config.ReceiveEndpoint(queueName, receiveBuilder =>
                    {
                        ConfigureEndPoint(receiveBuilder, queueName, consumerType);
                    });
                }

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                { 
                    receiveBuilder.UseMessageRetry(retry =>
                    {
                        retry.Interval(
                            messaging.MessageRetryInterval,
                            messaging.MessageRetryTimespan);
                    });

                    receiveBuilder.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Intervals(
                            TimeSpan.FromSeconds(1), 
                            TimeSpan.FromSeconds(10), 
                            TimeSpan.FromSeconds(20));
                    });

                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return services;
    }
}

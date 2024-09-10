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
            x.AddConsumer<ItemCreatedIntegrationEventConsumer>();
            x.AddConsumer<ItemUpdatedIntegrationEventConsumer>();
            x.AddConsumer<ItemDeletedIntegrationEventConsumer>();
            x.AddConsumer<ItemUndeletedIntegrationEventConsumer>();
            x.AddConsumer<ItemActivatedIntegrationEventConsumer>();
            x.AddConsumer<ItemInactivatedIntegrationEventConsumer>();
            x.AddConsumer<ItemPriceSetIntegrationEventConsumer>();
            x.AddConsumer<ItemSoldAddedIntegrationEventConsumer>();
            x.AddConsumer<ItemSoldReducedIntegrationEventConsumer>();
            x.AddConsumer<ItemSoldSetIntegrationEventConsumer>();
            x.AddConsumer<ItemStockAddedIntegrationEventConsumer>();
            x.AddConsumer<ItemStockReducedIntegrationEventConsumer>();
            x.AddConsumer<ItemStockSetIntegrationEventConsumer>();
            x.AddConsumer<BrandDeletedIntegrationEventConsumer>();
            x.AddConsumer<BrandUpdatedIntegrationEventConsumer>();
            x.AddConsumer<CategoryDeletedIntegrationEventConsumer>();
            x.AddConsumer<CategoryUpdatedIntegrationEventConsumer>();


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


                var consumers = new (string QueueName, Type ConsumerType)[]
                {
                    ("item-created-integration-event-queue", typeof(ItemCreatedIntegrationEventConsumer)),
                    ("item-updated-integration-event-queue", typeof(ItemUpdatedIntegrationEventConsumer)),
                    ("item-deleted-integration-event-queue", typeof(ItemDeletedIntegrationEventConsumer)),
                    ("item-undeleted-integration-event-queue", typeof(ItemUndeletedIntegrationEventConsumer)),
                    ("item-activated-integration-event-queue", typeof(ItemActivatedIntegrationEventConsumer)),
                    ("item-inactivated-integration-event-queue", typeof(ItemInactivatedIntegrationEventConsumer)),
                    ("item-price-set-integration-event-queue", typeof(ItemPriceSetIntegrationEventConsumer)),
                    ("item-sold-added-integration-event-queue", typeof(ItemSoldAddedIntegrationEventConsumer)),
                    ("item-sold-reduced-integration-event-queue", typeof(ItemSoldReducedIntegrationEventConsumer)),
                    ("item-sold-set-integration-event-queue", typeof(ItemSoldSetIntegrationEventConsumer)),
                    ("item-stock-added-integration-event-queue", typeof(ItemStockAddedIntegrationEventConsumer)),
                    ("item-stock-reduced-integration-event-queue", typeof(ItemStockReducedIntegrationEventConsumer)),
                    ("item-stock-set-integration-event-queue", typeof(ItemStockSetIntegrationEventConsumer)),
                    ("brand-deleted-integration-event-queue", typeof(BrandDeletedIntegrationEventConsumer)),
                    ("brand-updated-integration-event-queue", typeof(BrandUpdatedIntegrationEventConsumer)),
                    ("category-deleted-integration-event-queue", typeof(CategoryDeletedIntegrationEventConsumer)),
                    ("category-updated-integration-event-queue", typeof(CategoryUpdatedIntegrationEventConsumer))
                };

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

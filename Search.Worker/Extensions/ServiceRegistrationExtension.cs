using Core.Configurations;
using MassTransit;  
using Search.Worker.Consumers;
using Search.Worker.Domain.Repositories;
using Search.Worker.Infrastructure;
using Search.Worker.Infrastructure.Repositories;

namespace Search.Worker.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    { 
        builder.Services.AddScoped<ElasticsearchContext>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblyContaining<ISearchWorkerMarkerInterface>();
        });

        return builder;
    }

    public static WebApplicationBuilder AddMasstransitContext(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>()
            ?? throw new Exception(); 

        builder.Services.AddMassTransit(x =>
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

            x.AddConsumersFromNamespaceContaining<ISearchWorkerMarkerInterface>();
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
                    receiveBuilder.UseScheduledRedelivery(redelivery => redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval)));
                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return builder;
    }
}

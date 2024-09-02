using MassTransit; 
using Search.Worker.Configurations;
using Search.Worker.Consumers;
using Search.Worker.Domain.Repositories;
using Search.Worker.Infrastructure;
using Search.Worker.Infrastructure.Repositories;

var builder = Host.CreateApplicationBuilder(args); 

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.Configure<ElasticsearchOptions>(builder.Configuration.GetSection("Elasticsearch"));

builder.Services.AddScoped<ElasticsearchContext>();

builder.Services.AddMassTransit(x =>
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
        config.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"] ?? "user");
            host.Password(builder.Configuration["RabbitMQ:Password"] ?? "user");
        });

        config.UseMessageRetry(retryCfg =>
        {
            retryCfg.Interval(3,
                TimeSpan.FromSeconds(3));
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

        foreach (var (queueName , consumerType) in consumers)
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
                retry.Interval(3, TimeSpan.FromSeconds(2));
            });

            receiveBuilder.UseDelayedRedelivery(redelivery =>
            {
                redelivery.Intervals(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
            });

            receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
        }
    }); 
});

builder.Services.AddScoped<IItemRepository, ItemRepository>();

var host = builder.Build();
host.Run();

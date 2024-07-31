using MassTransit; 
using Search.Worker.Consumers;
using Search.Worker.Domain.Repository; 
using Search.Worker.Infrastructure.Repository;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("Elasticsearch", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Elasticsearch:Address"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


builder.Services.AddMassTransit(x =>
{      
    x.AddConsumer<ItemCreatedIntegrationEventConsumer>();
    x.AddConsumer<ItemUpdatedIntegrationEventConsumer>();
    x.AddConsumer<ItemDeletedIntegrationEventConsumer>(); 
    x.AddConsumer<ItemPriceSetIntegrationEventConsumer>(); 
    x.AddConsumer<ItemSoldAddedIntegrationEventConsumer>(); 
    x.AddConsumer<ItemSoldReducedIntegrationEventConsumer>(); 
    x.AddConsumer<ItemSoldSetIntegrationEventConsumer>(); 
    x.AddConsumer<ItemStockAddedIntegrationEventConsumer>(); 
    x.AddConsumer<ItemStockReducedIntegrationEventConsumer>(); 
    x.AddConsumer<ItemStockSetIntegrationEventConsumer>(); 
    
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"] ?? "user");
            host.Password(builder.Configuration["RabbitMQ:Password"] ?? "user");
        });


        var consumers = new (string QueueName, Type ConsumerType)[]
        {
            ("item-created-integration-event-queue", typeof(ItemCreatedIntegrationEventConsumer)),
            ("item-updated-integration-event-queue", typeof(ItemUpdatedIntegrationEventConsumer)),
            ("item-deleted-integration-event-queue", typeof(ItemDeletedIntegrationEventConsumer)),
            ("item-price-set-integration-event-queue", typeof(ItemPriceSetIntegrationEventConsumer)),
            ("item-sold-added-integration-event-queue", typeof(ItemSoldAddedIntegrationEventConsumer)),
            ("item-sold-reduced-integration-event-queue", typeof(ItemSoldReducedIntegrationEventConsumer)),
            ("item-sold-set-integration-event-queue", typeof(ItemSoldSetIntegrationEventConsumer)),
            ("item-stock-added-integration-event-queue", typeof(ItemStockAddedIntegrationEventConsumer)),
            ("item-stock-reduced-integration-event-queue", typeof(ItemStockReducedIntegrationEventConsumer)),
            ("item-stock-set-integration-event-queue", typeof(ItemStockSetIntegrationEventConsumer))
        };

        foreach(var (queueName , consumerType) in consumers)
        {
            config.ReceiveEndpoint(queueName, receiveBuilder =>
            {
                ConfigureEndPoint(receiveBuilder, queueName, consumerType);
            });
        }

        void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
        {
            receiveBuilder.ConfigureConsumer(context, consumerType);

            receiveBuilder.UseMessageRetry(retry =>
            {
                retry.Immediate(3);
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

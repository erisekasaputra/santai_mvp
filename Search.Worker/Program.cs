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

    x.AddConsumer<FaultItemCreatedEventConsumer>();
    x.AddConsumer<FaultItemUpdatedEventConsumer>();
    x.AddConsumer<FaultItemDeletedEventConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {    
        config.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"] ?? "user");
            host.Password(builder.Configuration["RabbitMQ:Password"] ?? "user");
        });  

        
        config.ReceiveEndpoint("item-created-integration-event-queue", receiveBuilder =>
        {   
            receiveBuilder.ConfigureConsumer<ItemCreatedIntegrationEventConsumer>(context);

            receiveBuilder.UseMessageRetry(retry =>
            {
                retry.Immediate(3);
            });
             
            receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2)); 
        });

        config.ReceiveEndpoint("item-created-integration-event-queue_error", receiveBuilder =>
        {
            receiveBuilder.ConfigureConsumer<FaultItemCreatedEventConsumer>(context);  
        });


        config.ReceiveEndpoint("item-updated-integration-event-queue", receiveBuilder =>
        {
            receiveBuilder.ConfigureConsumer<ItemUpdatedIntegrationEventConsumer>(context);

            receiveBuilder.UseMessageRetry(retry =>
            {
                retry.Immediate(3);
            });

            receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
        });

        config.ReceiveEndpoint("item-updated-integration-event-queue_error", receiveBuilder =>
        {
            receiveBuilder.ConfigureConsumer<FaultItemUpdatedEventConsumer>(context);
        });


        config.ReceiveEndpoint("item-deleted-integration-event-queue", receiveBuilder =>
        {
            receiveBuilder.ConfigureConsumer<ItemDeletedIntegrationEventConsumer>(context);

            receiveBuilder.UseMessageRetry(retry =>
            {
                retry.Immediate(3);
            });

            receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
        });

        config.ReceiveEndpoint("item-deleted-integration-event-queue_error", receiveBuilder =>
        {
            receiveBuilder.ConfigureConsumer<FaultItemDeletedEventConsumer>(context);
        });



        config.UseDelayedRedelivery(redelivery =>
        {
            redelivery.Intervals(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
        });

        config.ReceiveEndpoint();
    }); 
});

builder.Services.AddScoped<IItemRepository, ItemRepository>();

var host = builder.Build();
host.Run();

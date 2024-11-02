using Amazon.DynamoDBv2;
using Chat.API.Applications.Consumers;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Configurations;
using MassTransit;
using StackExchange.Redis;

namespace Chat.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
         
        var options = builder.Configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() ?? throw new Exception();

        builder.Services.AddSignalR().AddStackExchangeRedis(configure =>
        {
            var configurations = new ConfigurationOptions
            {
                EndPoints = { options.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                   .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds),
                ChannelPrefix = RedisChannel.Literal("ChatAPI")
            };
             
            configurations.Ssl = options.Ssl;

            configure.Configuration = configurations;
        });

        builder.Services.AddAWSService<IAmazonDynamoDB>();
        builder.Services.AddSingleton<IChatService, DynamoDBChatService>();




        var messagingOption = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>() ?? throw new Exception();
        builder.Services.AddMassTransit(x =>
        {   
            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("chat-service-order-create-integration-event-queue", typeof(OrderCreatedIntegrationEventConsumer))
            };

            x.AddConsumersFromNamespaceContaining<IChatAPIMarkerInterface>();
            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messagingOption.Host ?? string.Empty, host =>
                {
                    host.Username(messagingOption.Username ?? string.Empty);
                    host.Password(messagingOption.Password ?? string.Empty);
                });

                configure.UseTimeout(timeoutCfg => timeoutCfg.Timeout = TimeSpan.FromSeconds(messagingOption.MessageTimeout));

                foreach (var (queueName, consumerType) in consumers)
                    configure.ReceiveEndpoint(queueName, receiveBuilder => ConfigureEndPoint(receiveBuilder, queueName, consumerType));

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                {
                    receiveBuilder.ConfigureConsumer(context, consumerType);
                    receiveBuilder.UseMessageRetry(retry => retry.Interval(messagingOption.MessageRetryInterval, TimeSpan.FromSeconds(messagingOption.MessageRetryTimespan)));
                    receiveBuilder.UseScheduledRedelivery(redelivery => redelivery.Intervals(TimeSpan.FromSeconds(messagingOption.DelayedRedeliveryInterval)));
                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        }); 
        return builder;
    }
}

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Chat.API.Applications.Consumers;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Configurations;
using Core.Services;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Chat.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();

        builder.Services.AddScoped<IUserInfoService, UserInfoService>();

        var options = builder.Configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() ?? throw new Exception(); 
        builder.Services.AddSignalR(configure =>
        {
            configure.KeepAliveInterval = TimeSpan.FromMinutes(10);
            configure.ClientTimeoutInterval = TimeSpan.FromHours(24);
        }).AddStackExchangeRedis(configure =>
        {
            var configurations = new ConfigurationOptions
            {
                EndPoints = { options.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                   .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds),
                ChannelPrefix = RedisChannel.Literal("ChatAPI"),
                Ssl = options.Ssl
            };

            configure.Configuration = configurations;
        }); 






        var messagingOption = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>() ?? throw new Exception();
        builder.Services.AddMassTransit(x =>
        {   
            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("chat-service-order-created-integration-event-queue", typeof(OrderCreatedIntegrationEventConsumer)),
                ("chat-service-order-cancelled-by-buyer-integration-event-queue", typeof(OrderCancelledByBuyerIntegrationEventConsumer)),
                ("chat-service-order-cancelled-by-mechanic-integration-event-queue", typeof(OrderCancelledByMechanicIntegrationEventConsumer)),
                ("chat-service-account-mechanic-order-accepted-integration-event-queue", typeof(AccountMechanicOrderAcceptedIntegrationEventConsumer)),
                ("chat-service-service-completed-integration-event-queue", typeof(ServiceCompletedIntegrationEventConsumer)),
                ("chat-service-service-incompleted-integration-event-queue", typeof(ServiceIncompletedIntegrationEventConsumer)),
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

         
        builder.Services.AddSingleton<IAmazonDynamoDB>(provider =>
        { 
            var awsOptions = provider.GetRequiredService<IOptionsMonitor<AWSIAMConfiguration>>().CurrentValue;
             
            var credentials = new BasicAWSCredentials(awsOptions.AccessID, awsOptions.SecretKey);
             
            var region = RegionEndpoint.GetBySystemName(awsOptions.Region);
            return new AmazonDynamoDBClient(credentials, region);
        });
         
        builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>(); 
        builder.Services.AddScoped<IChatService, DynamoDBChatService>();

        return builder;
    }
}

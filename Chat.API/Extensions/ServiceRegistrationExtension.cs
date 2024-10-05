using Amazon.DynamoDBv2;
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
            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messagingOption.Host ?? string.Empty, host =>
                { 
                    host.Username(messagingOption.Username ?? string.Empty);
                    host.Password(messagingOption.Password ?? string.Empty);
                });

                configure.UseTimeout(timeoutCfg => timeoutCfg.Timeout = TimeSpan.FromSeconds(messagingOption.MessageTimeout)); 
            });
        });



        return builder;
    }
}

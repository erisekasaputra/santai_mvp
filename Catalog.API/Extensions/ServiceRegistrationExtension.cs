using Catalog.API.Applications.Consumers;
using Catalog.API.Applications.Services;
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Helpers;
using Core.Configurations;
using Core.Events;
using Core.Services;
using Core.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Catalog.API.Extensions;

public static class ServiceRegistrationExtension
{  
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    { 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddSingleton<ICacheService, CacheService>(); 
        services.AddScoped<ApplicationService>(); 
        services.AddScoped<MetaTableHelper>(); 
        return services;
    } 

    public static IServiceCollection AddMassTransitContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var messagingConfiguration = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        var messaging = messagingConfiguration.CurrentValue;

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.QueryTimeout = TimeSpan.FromSeconds(30);
                o.QueryMessageLimit = 100;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("catalog-service-order-failed-recovery-stock-integration-event-queue", typeof(OrderFailedRecoveryStockIntegrationEventConsumer))
            };

            foreach ((_, Type consumerType) in consumers) 
            {
                x.AddConsumer(consumerType);
            }

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messaging.Host
                    ?? throw new Exception(nameof(messaging.Host)), host =>
                {
                    host.Username(messaging.Username
                    ?? throw new Exception(nameof(messaging.Host)));
                    host.Password(messaging.Password
                    ?? throw new Exception(nameof(messaging.Host)));
                }); 

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(
                        messaging.MessageRetryInterval,
                        messaging.MessageRetryTimespan);
                }); 

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(5);
                });

                configure.ConfigureEndpoints(context); 
               
                foreach (var (queueName, consumerType) in consumers)
                {
                    configure.ReceiveEndpoint(queueName, receiveBuilder =>
                    {
                        ConfigureEndPoint(receiveBuilder, queueName, consumerType);
                    });
                }

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                {
                    receiveBuilder.UseMessageRetry(retry =>
                    {
                        retry.Interval(messaging.MessageRetryInterval, TimeSpan.FromSeconds(messaging.MessageRetryTimespan));
                    });

                    receiveBuilder.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Intervals(TimeSpan.FromSeconds(messaging.DelayedRedeliveryInterval));
                    });

                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return services;
    } 
}

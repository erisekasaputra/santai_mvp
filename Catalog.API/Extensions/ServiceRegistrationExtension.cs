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

        var options = messagingConfiguration.CurrentValue;

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

            x.AddConsumersFromNamespaceContaining<ICatalogAPIMarkerInterface>();
            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseTimeout(timeoutCfg => timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout));
                //configure.ConfigureEndpoints(context);

                foreach (var (queueName, consumerType) in consumers)
                    configure.ReceiveEndpoint(queueName, receiveBuilder => ConfigureEndPoint(receiveBuilder, queueName, consumerType));

                void ConfigureEndPoint(IReceiveEndpointConfigurator receiveBuilder, string queueName, Type consumerType)
                {
                    receiveBuilder.ConfigureConsumer(context, consumerType);
                    receiveBuilder.UseMessageRetry(retry => retry.Interval(options.MessageRetryInterval, TimeSpan.FromSeconds(options.MessageRetryTimespan)));
                    receiveBuilder.UseDelayedRedelivery(redelivery => redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval)));
                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });

        return services;
    } 
}

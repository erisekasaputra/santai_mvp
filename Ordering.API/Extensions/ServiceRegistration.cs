using Microsoft.Extensions.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Core.Configurations;
using Core.Services;
using Core.Services.Interfaces;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure;
using Ordering.API.CustomDelegates;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Applications.Services;
using Polly.Extensions.Http;
using Polly;

namespace Ordering.API.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddTransient<TokenJwtHandler>();
        services.AddScoped<IPaymentService, PaymentService>(); 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection service)
    { 
        var accountConfig = service.BuildServiceProvider().GetService<IOptionsMonitor<AccountServiceConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        var catalogConfig = service.BuildServiceProvider().GetService<IOptionsMonitor<CatalogServiceConfiguration>>()
           ?? throw new Exception("Please provide value for database option");


        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));



        service.AddHttpClient<IAccountServiceAPI, AccountServiceAPI>(client =>
        {
            client.BaseAddress = new Uri(accountConfig?.CurrentValue.Host ?? throw new Exception("Account service client host is not set"));
        }) 
        .AddPolicyHandler(retryPolicy)
        .AddHttpMessageHandler<TokenJwtHandler>();

        service.AddHttpClient<ICatalogServiceAPI, CatalogServiceAPI>(client =>
        {
            client.BaseAddress = new Uri(catalogConfig?.CurrentValue.Host ?? throw new Exception("Account service client host is not set"));
        })
       .AddPolicyHandler(retryPolicy)
       .AddHttpMessageHandler<TokenJwtHandler>();

        return service;
    }

    public static IServiceCollection AddMassTransitContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        var options = messageBusOptions.CurrentValue;

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(options.DuplicateDetectionWindows);
                o.QueryDelay = TimeSpan.FromSeconds(options.QueryDelay);
                o.QueryTimeout = TimeSpan.FromSeconds(options.QueryTimeout);
                o.QueryMessageLimit = options.QueryMessageLimit;
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.AddConsumersFromNamespaceContaining<IOrderAPIMarkerInterface>();

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(options.Host ?? string.Empty, host =>
                {
                    host.Username(options.Username ?? string.Empty);
                    host.Password(options.Password ?? string.Empty);
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(options.MessageRetryInterval,
                        TimeSpan.FromSeconds(options.MessageRetryTimespan));
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(options.MessageTimeout);
                });

                configure.ConfigureEndpoints(context);

                var consumers = new (string QueueName, Type ConsumerType)[]
                {
                    //("identity-email-assigned-to-a-user-integration-event-queue", typeof(IdentityEmailAssignedToAUserIntegrationEventConsumer))
                };

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
                        retry.Interval(options.MessageRetryInterval, TimeSpan.FromSeconds(options.MessageRetryTimespan));
                    });

                    receiveBuilder.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval));
                    });

                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });
        return services;
    }
}

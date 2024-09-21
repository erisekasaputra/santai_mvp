using Microsoft.Extensions.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Core.Configurations;
using Core.Services;
using Core.Services.Interfaces;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure;
using Ordering.API.Applications.Services.Interfaces;
using Ordering.API.Applications.Services;
using Polly.Extensions.Http;
using Polly;
using Ordering.API.Applications.Consumers;
using Core.CustomDelegates;

namespace Ordering.API.Extensions;

public static class ServiceRegistrationExtension
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITokenService, JwtTokenService>();
        builder.Services.AddTransient<TokenJwtHandler>();
        builder.Services.AddScoped<IPaymentService, PaymentService>(); 
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserInfoService, UserInfoService>();
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddHostedService<ScheduledOrderWorker>();
        return builder;
    }

    public static WebApplicationBuilder AddHttpClients(this WebApplicationBuilder builder)
    { 
        var accountOptions = builder.Configuration.GetSection(AccountServiceConfiguration.SectionName).Get<AccountServiceConfiguration>() ?? throw new Exception();
        var catalogOptions = builder.Configuration.GetSection(CatalogServiceConfiguration.SectionName).Get<CatalogServiceConfiguration>() ?? throw new Exception();
        var masterOptions = builder.Configuration.GetSection(MasterDataServiceConfiguration.SectionName).Get<MasterDataServiceConfiguration>() ?? throw new Exception(); 

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt))); 

        builder.Services.AddHttpClient<IAccountServiceAPI, AccountServiceAPI>(client =>
        {
            client.BaseAddress = new Uri(accountOptions.Host ?? throw new Exception("Account service client host is not set"));
        })
        .AddPolicyHandler(retryPolicy).AddHttpMessageHandler<TokenJwtHandler>();

          


        builder.Services.AddHttpClient<ICatalogServiceAPI, CatalogServiceAPI>(client =>
        {
            client.BaseAddress = new Uri(catalogOptions?.Host ?? throw new Exception("Catalog service client host is not set"));
        })
        .AddPolicyHandler(retryPolicy).AddHttpMessageHandler<TokenJwtHandler>();



        builder.Services.AddHttpClient<IMasterDataServiceAPI, MasterDataServiceAPI>(client =>
        {
            client.BaseAddress = new Uri(masterOptions.Host ?? throw new Exception("Master data service client host is not set"));
        })
       .AddPolicyHandler(retryPolicy);



        return builder;
    }

    public static WebApplicationBuilder AddMassTransitContext<TDbContext>(this WebApplicationBuilder builder) where TDbContext : DbContext
    {
        var options = builder.Configuration.GetSection(MessagingConfiguration.SectionName).Get<MessagingConfiguration>() ?? throw new Exception();  
        builder.Services.AddMassTransit(x =>
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

            var consumers = new (string QueueName, Type ConsumerType)[]
            {
                ("ordering-service-account-mechanic-order-accepted-integration-event-queue", typeof(AccountMechanicOrderAcceptedIntegrationEventConsumer))
            };

            x.AddConsumersFromNamespaceContaining<IOrderAPIMarkerInterface>();
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
                    receiveBuilder.UseDelayedRedelivery(redelivery => redelivery.Intervals(TimeSpan.FromSeconds(options.DelayedRedeliveryInterval)));
                    receiveBuilder.UseRateLimit(1000, TimeSpan.FromSeconds(2));
                }
            });
        });
        return builder;
    }
}

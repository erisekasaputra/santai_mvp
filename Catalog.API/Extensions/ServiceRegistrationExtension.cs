using Catalog.API.Applications.Services; 
using Catalog.Domain.SeedWork;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Helpers;
using Core.Configurations; 
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
        var messageBusOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<MessagingConfiguration>>()
            ?? throw new Exception("Please provide value for message bus options");

        var options = messageBusOptions.CurrentValue;

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

            //x.AddConsumersFromNamespaceContaining<ICatalogAPIMarkerInterface>();

            x.UsingRabbitMq((context, configure) =>
            {
                configure.Host(messageBusOptions.CurrentValue.Host
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)), host =>
                {
                    host.Username(messageBusOptions.CurrentValue.Username
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)));
                    host.Password(messageBusOptions.CurrentValue.Password
                    ?? throw new Exception(nameof(messageBusOptions.CurrentValue.Host)));
                });

                configure.UseMessageRetry(retryCfg =>
                {
                    retryCfg.Interval(3, TimeSpan.FromSeconds(2));
                });

                configure.UseTimeout(timeoutCfg =>
                {
                    timeoutCfg.Timeout = TimeSpan.FromSeconds(5);
                });

                configure.ConfigureEndpoints(context);
            });
        });

        return services;
    } 
}

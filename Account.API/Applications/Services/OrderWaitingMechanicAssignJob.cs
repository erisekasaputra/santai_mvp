using Account.API.Applications.Services.Interfaces;
using Account.Domain.Events;
using Account.Domain.SeedWork;
using Core.Configurations; 
using MediatR;
using Microsoft.Extensions.Options;
using System.Data;

namespace Account.API.Applications.Services; 
 
public class OrderWaitingMechanicAssignJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory; 
    private readonly ILogger<OrderWaitingMechanicAssignJob> _logger;
    private IMechanicCache _mechanicCache;

    public OrderWaitingMechanicAssignJob(
        IServiceScopeFactory serviceScopeFactory, 
        ILogger<OrderWaitingMechanicAssignJob> logger)
    {
        _mechanicCache = null!;
        _scopeFactory = serviceScopeFactory; 
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        while (!stoppingToken.IsCancellationRequested)
        { 
            using var scope = _scopeFactory.CreateScope(); 
            _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>(); 

            var isShutdown = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<SafelyShutdownConfiguration>>(); 
            if (isShutdown.CurrentValue.Shutdown)
            { 
                await Task.Delay(10000);
                continue;
            }


            (bool isSuccess, string orderId, string buyerId, string mechanicId) = await _mechanicCache.ProcessOrdersWaitingMechanicAssignFromQueueAsync();

            if (isSuccess && !string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(buyerId) && !string.IsNullOrEmpty(mechanicId))
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadUncommitted, stoppingToken);

                await mediator.Publish(new AccountMechanicAutoSelectedToAnOrderDomainEvent(Guid.Parse(orderId), Guid.Parse(buyerId), Guid.Parse(mechanicId)), stoppingToken);

                await unitOfWork.CommitTransactionAsync(stoppingToken);
            } 
            await Task.Delay(500, stoppingToken);
        }
    }
} 
 
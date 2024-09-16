using Account.API.Applications.Services.Interfaces;  
using Core.Utilities;  

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
        while (true)
        {
            using var scope = _scopeFactory.CreateScope(); 
            _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>(); 
            await _mechanicCache.ProcessOrdersWaitingMechanicAssignFromQueueAsync();
            await Task.Delay(50000);
        }
    }
} 
 
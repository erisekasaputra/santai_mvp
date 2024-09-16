using Account.API.Applications.Services.Interfaces;  

namespace Account.API.Applications.Services;

public class OrderWaitingMechanicConfirmExpiryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderWaitingMechanicConfirmExpiryJob> _logger;
    private IMechanicCache _mechanicCache;

    public OrderWaitingMechanicConfirmExpiryJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OrderWaitingMechanicConfirmExpiryJob> logger)
    {
        _scopeFactory = serviceScopeFactory;
        _mechanicCache = null!;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    _mechanicCache = scope.ServiceProvider.GetRequiredService<IMechanicCache>();
        //    await _mechanicCache.ProcessOrdersWaitingMechanicConfirmExpiryFromQueueAsync();
        //    await Task.Delay(5000, stoppingToken);
        //}
    }
}
 
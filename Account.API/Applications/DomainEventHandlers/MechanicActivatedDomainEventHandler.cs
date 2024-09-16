using Account.API.Applications.Models;
using Account.API.Applications.Services.Interfaces;
using Account.Domain.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicActivatedDomainEventHandler(
    IMechanicCache cache) : INotificationHandler<MechanicActivatedDomainEvent>
{
    private readonly IMechanicCache _cache = cache;

    public async Task Handle(MechanicActivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var mechanic = new MechanicExistence()
        {
            MechanicId = notification.User.MechanicId,
            OrderId = null,
            Latitude = 0,
            Longitude = 0,  
        };

        await _cache.Ping();
        await _cache.CreateGeoAsync(mechanic);
        await _cache.CreateMechanicHsetAsync(mechanic);
    }
}

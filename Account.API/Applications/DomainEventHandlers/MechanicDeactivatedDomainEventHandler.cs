using Account.API.Applications.Services.Interfaces;
using Account.Domain.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicDeactivatedDomainEventHandler(
    IMechanicCache cache) : INotificationHandler<MechanicDeactivatedDomainEvent>
{
    private readonly IMechanicCache _cache = cache;
    public async Task Handle(MechanicDeactivatedDomainEvent request, CancellationToken cancellationToken)
    {
        await _cache.Ping();
        await _cache.RemoveGeoAsync(request.User.MechanicId);
        await _cache.RemoveHsetAsync(request.User.MechanicId);
    }
}

using Catalog.Domain.Events;
using MediatR;

namespace Catalog.API.IntegrationEvents.EventHandlers.Outgoing;

public class ItemCreatedIntegrationEventHandler : INotificationHandler<ItemCreatedDomainEvent>
{
    public Task Handle(ItemCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine(notification.Item.Id);
        return Task.CompletedTask;
    }
}

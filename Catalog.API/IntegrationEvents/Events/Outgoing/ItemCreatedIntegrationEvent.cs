using Catalog.Domain.Aggregates.ItemAggregate;

namespace Catalog.API.IntegrationEvents.Events.Outgoing;

public class ItemCreatedIntegrationEvent(Item item)
{
    public Item Item { get; set; } = item;
}

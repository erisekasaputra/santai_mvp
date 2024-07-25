using Catalog.Domain.Aggregates.ItemAggregate;
using MediatR;

namespace Catalog.Domain.Events;

public class ItemCreatedDomainEvent(Item item) : INotification
{
    public Item Item { get; set; } = item;
}

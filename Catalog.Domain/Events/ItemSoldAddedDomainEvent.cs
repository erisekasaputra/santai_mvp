using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldAddedDomainEvent(string Id, int Quantity) : INotification; 

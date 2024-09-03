using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldAddedDomainEvent(Guid Id, int Quantity) : INotification; 

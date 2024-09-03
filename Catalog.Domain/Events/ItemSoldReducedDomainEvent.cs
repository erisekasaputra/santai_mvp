using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldReducedDomainEvent(Guid Id, int Quantity) : INotification;

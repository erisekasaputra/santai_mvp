using MediatR;

namespace Catalog.Domain.Events;

public record ItemSoldReducedDomainEvent(string Id, int Quantity) : INotification;

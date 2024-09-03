using MediatR;

namespace Catalog.Contracts;

public record ItemSoldReducedIntegrationEvent(Guid Id, int Quantity) : INotification;

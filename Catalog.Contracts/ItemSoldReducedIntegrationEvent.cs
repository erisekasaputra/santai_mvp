using MediatR;

namespace Catalog.Contracts;

public record ItemSoldReducedIntegrationEvent(string Id, int Quantity) : INotification;

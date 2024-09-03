using MediatR;

namespace Catalog.Contracts;

public record ItemSoldSetIntegrationEvent(Guid Id, int 
    Quantity) : INotification;

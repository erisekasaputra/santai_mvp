using MediatR;

namespace Account.Domain.Events;

public record MechanicUserDeletedDomainEvent(Guid Id) : INotification; 

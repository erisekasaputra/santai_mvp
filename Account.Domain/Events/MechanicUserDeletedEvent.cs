using MediatR;

namespace Account.Domain.Events;

public record MechanicUserDeletedEvent(Guid Id) : INotification; 

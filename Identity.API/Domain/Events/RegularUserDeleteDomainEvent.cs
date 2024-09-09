using MediatR;

namespace Identity.API.Domain.Events;

public record RegularUserDeleteDomainEvent(Guid UserId) : INotification; 

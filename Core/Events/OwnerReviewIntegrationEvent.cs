using MediatR;

namespace Core.Events;

public record OwnerReviewIntegrationEvent(string Title, int Rating) : INotification;

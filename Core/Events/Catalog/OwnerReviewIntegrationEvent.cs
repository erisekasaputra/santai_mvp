using MediatR;

namespace Core.Events.Catalog;

public record OwnerReviewIntegrationEvent(string Title, int Rating) : INotification;

using MediatR;

namespace Catalog.Contracts;

public record OwnerReviewIntegrationEvent(string Title, int Rating) : INotification;

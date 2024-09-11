using MediatR;

namespace Core.Events;

public record OrderPaymentPaidIntegrationEvent : INotification;

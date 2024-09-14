using Core.Events;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderPaymentPaidIntegrationEventHandler : INotificationHandler<OrderPaymentPaidIntegrationEvent>
{
    public async Task Handle(OrderPaymentPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

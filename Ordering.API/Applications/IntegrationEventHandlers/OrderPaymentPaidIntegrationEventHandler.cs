using Core.Events; 
using MassTransit;
using MediatR;

namespace Ordering.API.Applications.IntegrationEventHandlers;

public class OrderPaymentPaidIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<OrderPaymentPaidIntegrationEvent>
{
    private readonly IPublishEndpoint _endpoint = publishEndpoint;
    public async Task Handle(OrderPaymentPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _endpoint.Publish(notification, cancellationToken);
    }
}

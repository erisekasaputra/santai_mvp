using Core.Events.Chat;
using MassTransit;
using MediatR;

namespace Chat.API.Applications.IntegrationEventHandler;

public class ChatSentIntegrationEventHandler(IPublishEndpoint publishEndpoint) : INotificationHandler<ChatSentIntegrationEvent>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    public async Task Handle(ChatSentIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}

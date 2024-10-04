using Chat.API.Domain.Events;
using Core.Events.Chat;
using MediatR;

namespace Chat.API.Applications.DomainEventHandler;

public class ChatSentDomainEventHandler(IMediator mediator) : INotificationHandler<ChatSentDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Handle(ChatSentDomainEvent notification, CancellationToken cancellationToken)
    {
        await _mediator.Publish(
            new ChatSentIntegrationEvent(
                notification.OriginUserId,
                notification.DestinationUserId,
                notification.MessageId,
                notification.Text,
                notification.Timestamp), cancellationToken);
    }
}

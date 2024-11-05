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
                Guid.Parse(notification.Conversation.MessageId),
                Guid.Parse(notification.Conversation.OrderId),
                Guid.Parse(notification.Conversation.OriginUserId),
                Guid.Parse(notification.Conversation.DestinationUserId),
                notification.Conversation.Text,
                notification.Conversation.Attachment,
                string.IsNullOrEmpty(notification.Conversation.ReplyMessageId) ? null : Guid.Parse(notification.Conversation.ReplyMessageId),
                notification.Conversation.ReplyMessageText,
                notification.Conversation.Timestamp), cancellationToken);
    }
}

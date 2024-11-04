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
                notification.Conversation.MessageId,
                notification.Conversation.OrderId,
                notification.Conversation.OriginUserId,
                notification.Conversation.DestinationUserId,
                notification.Conversation.Text,
                notification.Conversation.Attachment,
                notification.Conversation.ReplyMessageId,
                notification.Conversation.ReplyMessageText,
                notification.Conversation.Timestamp), cancellationToken);
    }
}

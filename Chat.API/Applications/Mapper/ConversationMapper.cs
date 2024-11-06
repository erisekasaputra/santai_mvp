using Chat.API.Applications.Dtos.Response;
using Chat.API.Domain.Models;

namespace Chat.API.Applications.Mapper;

public static class ConversationMapper
{
    public static ConversationResponse ToResponse(this Conversation conversation)
    {
        return new ConversationResponse(
            conversation.MessageId,
            conversation.OrderId,
            conversation.OriginUserId,
            conversation.DestinationUserId,
            conversation.Text,
            conversation.Attachment ?? string.Empty,
            conversation.ReplyMessageId ?? string.Empty,
            conversation.ReplyMessageText ?? string.Empty,
            conversation.Timestamp);
    }
}

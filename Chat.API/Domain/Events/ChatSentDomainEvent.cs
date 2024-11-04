using Chat.API.Domain.Models;
using MediatR;

namespace Chat.API.Domain.Events;

public record ChatSentDomainEvent(Conversation Conversation) : INotification;

 
using Account.Domain.Events;
using Core.Events;
using Core.Services.Interfaces;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class MechanicUserCreatedDomainEventHandler(
    IMediator mediator,
    IEncryptionService kmsClient) : INotificationHandler<MechanicUserCreatedDomainEvent>
{
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(MechanicUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = notification.MechanicUser;
        await _mediator.Publish(new MechanicUserCreatedIntegrationEvent(
            @event.Id, await DecryptNullableAsync(@event.EncryptedPhoneNumber)), cancellationToken);
    }

    private async Task<string?> DecryptNullableAsync(string? value)
    {
        if (value == null) return null;

        return await _kmsClient.DecryptAsync(value);
    }

    private async Task<string> DecryptAsync(string value)
    {
        return await _kmsClient.DecryptAsync(value);
    }
}

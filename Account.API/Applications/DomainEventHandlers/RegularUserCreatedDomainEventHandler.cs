using Account.API.Services;
using Account.Domain.Events;
using Identity.Contracts.IntegrationEvent;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class RegularUserCreatedDomainEventHandler(
    IMediator mediator,
    IKeyManagementService kmsClient) : INotificationHandler<RegularUserCreatedDomainEvent>
{
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(RegularUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = notification.RegularUser;
        await _mediator.Publish(new RegularUserCreatedIntegrationEvent(@event.Id, await DecryptNullableAsync(@event.EncryptedPhoneNumber)), cancellationToken);
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

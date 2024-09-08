using Account.API.Applications.Services.Interfaces;
using Account.Domain.Events;
using Core.Events;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class StaffCreatedDomainEventHandler(
    IMediator mediator,
    IKeyManagementService kmsClient) : INotificationHandler<StaffCreatedDomainEvent>
{
    private readonly IKeyManagementService _kmsClient = kmsClient;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(StaffCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = notification.Staff;

        var staffEvent = new StaffIntegrationEvent(@event.Id, @event.BusinessUserCode, await DecryptAsync(@event.EncryptedPhoneNumber!), await DecryptNullableAsync(@event.EncryptedEmail), @event.Name, @event.TimeZoneId, @event.Password);

        await _mediator.Publish(new StaffUserCreatedIntegrationEvent(staffEvent), cancellationToken);
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

 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Events;
using Core.Events;
using Core.Services.Interfaces;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class BusinessUserCreatedDomainEventHandler(
    IMediator mediator,
    IEncryptionService kmsClient) : INotificationHandler<BusinessUserCreatedDomainEvent>
{ 
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(BusinessUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var entity = notification.BusinessUser;

            var staffEvents = new List<StaffIntegrationEvent>();

            foreach (Staff staff in entity?.Staffs ?? [])
            {
                var staffEvent = new StaffIntegrationEvent(
                    staff.Id,
                    staff.BusinessUserCode,
                    await DecryptAsync(staff.EncryptedPhoneNumber!),
                    await DecryptNullableAsync(staff.EncryptedEmail),
                    staff.Name,
                    staff.TimeZoneId,
                    staff.Password);

                staffEvents.Add(staffEvent);
            }

            if (entity is null)
            {
                return;
            }

            var @event = new BusinessUserCreatedIntegrationEvent(
                entity.Id,
                await DecryptNullableAsync(entity.EncryptedEmail),
                await DecryptAsync(entity.EncryptedPhoneNumber ?? string.Empty),
                entity.TimeZoneId,
                entity.Code,
                entity.BusinessName,
                entity.EncryptedContactPerson,
                entity.EncryptedTaxId,
                entity.WebsiteUrl,
                entity.Description,
                entity.Password,
                staffEvents);

            await _mediator.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



    private async Task<string?> DecryptNullableAsync(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        return await _kmsClient.DecryptAsync(value);
    }

    private async Task<string> DecryptAsync(string value)
    {
        return await _kmsClient.DecryptAsync(value);
    }
}

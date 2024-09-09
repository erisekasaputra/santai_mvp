 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Events;
using Core.Events;
using Core.Services.Interfaces;
using MediatR;

namespace Account.API.Applications.DomainEventHandlers;

public class BusinessUserDeletedDomainEventHandler(
    IMediator mediator,
    IEncryptionService kmsClient) : INotificationHandler<BusinessUserDeletedDomainEvent>
{
    private readonly IEncryptionService _kmsClient = kmsClient;
    private readonly IMediator _mediator = mediator; 
    public async Task Handle(BusinessUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {  
        try
        {
            var entity = notification.BusinessUser;

            var staffEvents = new List<StaffIntegrationEvent>();

            foreach (Staff staff in entity?.Staffs ?? [])
            {
                var businessLicense = new StaffIntegrationEvent(
                    staff.Id,
                    staff.BusinessUserCode,
                    await DecryptAsync(staff.EncryptedPhoneNumber!),
                    await DecryptNullableAsync(staff.EncryptedEmail),
                    staff.Name,
                    staff.TimeZoneId,
                    staff.Password);

                staffEvents.Add(businessLicense);
            }

            if (entity is null)
            {
                return;
            }

            var @event = new BusinessUserDeletedIntegrationEvent(
                entity.Id,
                await DecryptNullableAsync(entity.EncryptedEmail),
                await DecryptAsync(entity.EncryptedPhoneNumber!),
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
        if (value == null) return null;

        return await _kmsClient.DecryptAsync(value);
    }

    private async Task<string> DecryptAsync(string value)
    {
        return await _kmsClient.DecryptAsync(value);
    }
}

using Account.API.Services;
using Account.Contracts.EventEntity; 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Events;
using MassTransit;
using MediatR;
using Account.Contracts.IntegrationEvents;

namespace Account.API.Applications.IntegrationEventHandlers;


public class BusinessUserCreatedIntegrationEventHandler(
    IPublishEndpoint publishEndpoint,
    IKeyManagementService kmsClient) : INotificationHandler<BusinessUserCreatedDomainEvent>
{
    private readonly IPublishEndpoint _publisheEndpoint = publishEndpoint;
    private readonly IKeyManagementService _kmsClient = kmsClient;
     
    public async Task Handle(BusinessUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var entity = notification.BusinessUser;

            var staffEvents = new List<StaffEvent>();

            foreach (Staff b in entity?.Staffs ?? [])
            {
                var businessLicense = new StaffEvent(
                    b.Id,
                    await DecryptNullableAsync(b.EncryptedPhoneNumber),
                    await DecryptNullableAsync(b.EncryptedEmail),
                    b.Name,
                    b.TimeZoneId);

                staffEvents.Add(businessLicense);
            }

            if (entity is null)
            {
                return;
            }

            var @event = new BusinessUserCreatedIntegrationEvent(
                    entity.Id,
                    await DecryptNullableAsync(entity.EncryptedEmail),
                    await DecryptNullableAsync(entity.EncryptedPhoneNumber),
                    entity.TimeZoneId,
                    entity.Code,
                    entity.BusinessName,
                    entity.EncryptedContactPerson,
                    entity.EncryptedTaxId,
                    entity.WebsiteUrl,
                    entity.Description,
                    staffEvents
                );

            await _publisheEndpoint.Publish(@event, cancellationToken); 
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
        return await _kmsClient.EncryptAsync(value);
    }
}
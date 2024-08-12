using Account.Contracts;
using Account.Contracts.EventEntity; 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Events;
using MassTransit;
using MediatR; 

namespace Account.API.Applications.DomainEventHandlers;

public class BusinessUserCreatedDomainEventHandler : INotificationHandler<BusinessUserCreatedDomainEvent>
{
    public async Task Handle(BusinessUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var entity = notification.BusinessUser;

        if (entity is null)
        {
            return;
        }

        var staffEvents = new List<StaffEvent>();

        if (entity?.Staffs?.Count > 0)
        {
            foreach (Staff b in entity.Staffs)
            {
                var businessLicense = new StaffEvent(b.Username, b.HashedPhoneNumber, b.HashedEmail, b.Name, b.TimeZoneId);
                staffEvents.Add(businessLicense);
            }
        }

        var @event = new BusinessUserCreatedIntegrationEvent(
                entity!.Id,
                entity.Username,
                entity.HashedEmail,
                entity.HashedPhoneNumber,
                entity.TimeZoneId,
                entity.BusinessName,
                entity.EncryptedContactPerson,
                entity.EncryptedTaxId,
                entity.WebsiteUrl,
                entity.Description,
                staffEvents
            );

        await Task.CompletedTask;
    }
}

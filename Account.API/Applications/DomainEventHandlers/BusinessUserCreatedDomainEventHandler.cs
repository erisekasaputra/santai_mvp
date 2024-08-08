using Account.Contracts;
using Account.Contracts.EventEntity; 
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Events;
using MassTransit;
using MediatR;
using System.Collections.ObjectModel;

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

        var staffEvents = new Collection<StaffEvent>();

        if (entity?.Staffs?.Count > 0)
        {
            foreach (Staff b in entity.Staffs)
            {
                var businessLicense = new StaffEvent(b.Username, b.PhoneNumber, b.Email, b.Name, b.TimeZoneId);
                staffEvents.Add(businessLicense);
            }
        }

        var @event = new BusinessUserCreatedIntegrationEvent(
                entity!.Id,
                entity.Username,
                entity.Email,
                entity.PhoneNumber,
                entity.TimeZoneId,
                entity.BusinessName,
                entity.ContactPerson,
                entity.TaxId,
                entity.WebsiteUrl,
                entity.Description,
                staffEvents
            );

        await Task.CompletedTask;
    }
}

using Account.Domain.ValueObjects;
using MediatR;

namespace Account.Domain.Events;

public record BusinessUserUpdatedDomainEvent(
    Guid Id,
    string BusinessName,
    string ContactPerson,
    string ?TaxId,
    string ?WebsiteUrl,
    string ?Description,
    Address Address,
    string TimeZoneId) : INotification;

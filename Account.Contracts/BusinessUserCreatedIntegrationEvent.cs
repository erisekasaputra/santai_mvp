using Account.Contracts.EventEntity;
using MediatR;

namespace Account.Contracts;

public record BusinessUserCreatedIntegrationEvent(
    Guid UserId, 
    string? Email,
    string PhoneNumber,
    string TimeZoneId, 
    string BusinessName,
    string ContactPerson,
    string? TaxId,
    string? WebsiteUrl,
    string? BusinessDescription,  
    IEnumerable<StaffEvent>? Staffs
    ) : INotification;

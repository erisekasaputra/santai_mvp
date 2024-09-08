namespace Core.Events;

public record StaffIntegrationEvent(
    Guid Id,
    string BusinessCode,
    string PhoneNumber,
    string? Email,
    string Name,
    string TimeZoneId,
    string Password);
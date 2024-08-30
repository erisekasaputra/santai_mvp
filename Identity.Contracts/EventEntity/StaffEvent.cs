namespace Identity.Contracts.EventEntity;

public record StaffEvent(
    Guid Id,
    string? PhoneNumber,
    string? Email,
    string Name,
    string TimeZoneId);
namespace Identity.Contracts.EventEntity;

public record StaffEvent(
    Guid Id,
    string BusinessCode,
    string PhoneNumber,
    string? Email,
    string Name,
    string TimeZoneId,
    string Password);
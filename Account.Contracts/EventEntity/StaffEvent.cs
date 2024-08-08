namespace Account.Contracts.EventEntity;

public record StaffEvent(
    string Username,
    string PhoneNumber,
    string Email,
    string Name,
    string TimeZoneId);
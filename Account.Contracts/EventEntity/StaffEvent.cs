namespace Account.Contracts.EventEntity;

public record StaffEvent( 
    Guid id,
    string PhoneNumber,
    string? Email,
    string Name,
    string TimeZoneId);
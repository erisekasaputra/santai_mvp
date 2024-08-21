namespace Account.Contracts.EventEntity;

public record StaffEvent( 
    string PhoneNumber,
    string? Email,
    string Name,
    string TimeZoneId);
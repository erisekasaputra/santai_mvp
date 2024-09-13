namespace Account.API.Applications.Dtos.ResponseDtos;
public record StaffResponseDto(
    Guid Id,  
    string? Email,
    string? PhoneNumber,
    string Name,
    AddressResponseDto Address,
    string TimeZoneId,
    IEnumerable<FleetResponseDto> Fleets);
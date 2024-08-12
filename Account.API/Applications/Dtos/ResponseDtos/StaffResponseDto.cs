namespace Account.API.Applications.Dtos.ResponseDtos;
public record StaffResponseDto(
    Guid Id,
    string Username,
    string Email,
    string PhoneNumber,
    string Name,
    AddressResponseDto Address,
    string TimeZoneId);
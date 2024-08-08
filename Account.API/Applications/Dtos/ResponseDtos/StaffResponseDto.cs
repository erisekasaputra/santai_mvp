namespace Account.API.Applications.Dtos.ResponseDtos;
public record StaffResponseDto(
    Guid Id,
    string Username,
    string PhoneNumber,
    string Email,
    string Name,
    AddressResponseDto Address,
    string TimeZoneId);
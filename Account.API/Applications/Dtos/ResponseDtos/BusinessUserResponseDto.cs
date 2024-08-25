namespace Account.API.Applications.Dtos.ResponseDtos;

public record BusinessUserResponseDto(
    Guid UserId, 
    string? Email,
    string? PhoneNumber,
    string TimeZoneId,
    AddressResponseDto Address,
    string BusinessName,
    string ContactPerson,
    string? TaxId,
    string? WebsiteUrl,
    string? BusinessDescription,
    LoyaltyProgramResponseDto? Loyalty,
    IEnumerable<BusinessLicenseResponseDto>? BusinessLicenses,
    IEnumerable<StaffResponseDto>? Staffs);
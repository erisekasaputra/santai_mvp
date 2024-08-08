namespace Account.API.Applications.Dtos.ResponseDtos;

public record BusinessLicenseResponseDto(
    Guid Id,
    string LicenseNumber,
    string Name,
    string Description);

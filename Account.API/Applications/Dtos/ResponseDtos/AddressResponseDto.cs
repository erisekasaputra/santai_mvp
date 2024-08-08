namespace Account.API.Applications.Dtos.ResponseDtos;

public record AddressResponseDto(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string City,
    string State,
    string PostalCode,
    string Country);
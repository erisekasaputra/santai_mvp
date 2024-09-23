using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class AddressRequestDto(
    string addressLine1,
    string? addressLine2,
    string? addressLine3,
    string city,
    string state,
    string postalCode,
    string country)
{
    public required string AddressLine1 { get; set; } = addressLine1.Clean();
    public string? AddressLine2 { get; set; } = addressLine2?.Clean();
    public string? AddressLine3 { get; set; } = addressLine3?.Clean();
    public required string City { get; set; } = city.Clean();
    public required string State { get; set; } = state.Clean();
    public required string PostalCode { get; set; } = postalCode.Clean();
    public required string Country { get; set; } = country.Clean();
}
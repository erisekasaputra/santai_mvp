using Account.API.Extensions;

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
    public string AddressLine1 { get; set; } = addressLine1.Clean();
    public string? AddressLine2 { get; set; } = addressLine2?.Clean();
    public string? AddressLine3 { get; set; } = addressLine3?.Clean();
    public string City { get; } = city.Clean();
    public string State { get; } = state.Clean();
    public string PostalCode { get; } = postalCode.Clean();
    public string Country { get; } = country.Clean();
}
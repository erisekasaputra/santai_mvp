using Account.API.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class AddressRequestDto(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string City,
    string State,
    string PostalCode,
    string Country)
{
    public string AddressLine1 { get; } = AddressLine1.Clean();
    public string? AddressLine2 { get; } = AddressLine2?.Clean();
    public string? AddressLine3 { get; } = AddressLine3?.Clean();
    public string City { get; } = City.Clean();
    public string State { get; } = State.Clean();
    public string PostalCode { get; } = PostalCode.Clean();
    public string Country { get; } = Country.Clean();
}
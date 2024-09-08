using Account.API.Extensions;
using Core.Extensions;

namespace Account.API.Applications.Dtos.RequestDtos;

public class UpdateBusinessUserRequestDto(
    string businessName,
    string contactPerson,
    string? taxId,
    string? websiteUrl,
    string? description,
    AddressRequestDto address,
    string timeZoneId)
{ 
    public string BusinessName { get; } = businessName.Clean();
    public string ContactPerson { get; } = contactPerson.Clean();
    public string? TaxId { get; } = taxId?.Clean();
    public string? WebsiteUrl { get; } = websiteUrl?.Clean();
    public string? Description { get; } = description?.Clean();
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}

using Account.API.Applications.Dtos.RequestDtos;
using Account.API.Extensions;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateBusinessUserByUserId;

public class UpdateBusinessUserByUserIdCommand(
    Guid id,
    string businessName,
    string contactPerson,
    string? taxId,
    string? websiteUrl,
    string? description,
    AddressRequestDto address,
    string timeZoneId) : IRequest<Result>
{
    public Guid Id { get; set; } = id;
    public string BusinessName { get; } = businessName.Clean();
    public string ContactPerson { get; } = contactPerson.Clean();
    public string? TaxId { get; } = taxId?.Clean();
    public string? WebsiteUrl { get; } = websiteUrl?.Clean();
    public string? Description { get; } = description?.Clean();
    public AddressRequestDto Address { get; } = address;
    public string TimeZoneId { get; } = timeZoneId.Clean();
}

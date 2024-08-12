using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessUser;

public record CreateBusinessUserCommand(
        Guid IdentityId,
        string Username,
        string Email,
        string PhoneNumber,
        string TimeZoneId,
        AddressRequestDto Address,
        string BusinessName,
        string ContactPerson,
        string? TaxId,
        string? WebsiteUrl,
        string? BusinessDescription,
        string? ReferralCode,
        IEnumerable<BusinessLicenseRequestDto> BusinessLicenses,
        IEnumerable<StaffRequestDto> Staffs
    ) : IRequest<Result>;
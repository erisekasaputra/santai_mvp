using Account.API.Applications.Dtos.RequestDtos;
using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.BusinessUserCommand.CreateBusinessUser;

public record CreateBusinessUserCommand( 
        string? Email,
        string PhoneNumber,
        string TimeZoneId,
        AddressRequestDto Address,
        string BusinessName,
        string ContactPerson,
        string? TaxId,
        string? WebsiteUrl,
        string? BusinessDescription,
        string? ReferralCode,
        string Password,
        IEnumerable<BusinessLicenseRequestDto> BusinessLicenses,
        IEnumerable<StaffRequestDto> Staffs
    ) : IRequest<Result>;
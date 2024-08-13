using Account.API.Applications.Dtos.RequestDtos; 
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.CreateMechanicUser;

public record CreateMechanicUserCommand(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    string? ReferralCode,
    PersonalInfoRequestDto PersonalInfo,
    AddressRequestDto Address,
    IEnumerable<CertificationRequestDto> Certifications,
    DrivingLicenseRequestDto DrivingLicense,
    NationalIdentityRequestDto NationalIdentity,
    string DeviceId
    ) : IRequest<Result>;
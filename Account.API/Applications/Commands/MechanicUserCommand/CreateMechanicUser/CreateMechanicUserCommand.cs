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
    AddressRequestDto Address,
    IEnumerable<CertificationRequestDto> Certifications,
    DrivingLicenseRequestDto DrivingLicenseRequestDto,
    NationalIdentityRequestDto NationalIdentityRequestDto,
    string DeviceId
    ) : IRequest<Result>;
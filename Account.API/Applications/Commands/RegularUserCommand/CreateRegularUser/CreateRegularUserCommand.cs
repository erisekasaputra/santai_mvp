using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;

public record CreateRegularUserCommand(
    Guid IdentityId,
    string Username,
    string Email,
    string PhoneNumber,
    string TimeZoneId,
    AddressRequestDto Address,
    PersonalInfoRequestDto PersonalInfo,
    string DeviceId) : IRequest<Result>;

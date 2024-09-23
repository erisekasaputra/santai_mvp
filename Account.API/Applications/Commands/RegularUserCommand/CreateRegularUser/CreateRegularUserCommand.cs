using Account.API.Applications.Dtos.RequestDtos;
using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.CreateRegularUser;

public record CreateRegularUserCommand(
    Guid IdentityId, 
    string? Email,
    string PhoneNumber,
    string TimeZoneId,
    string? ReferralCode,
    AddressRequestDto Address,
    PersonalInfoRequestDto PersonalInfo ) : IRequest<Result>;

using Account.API.Applications.Dtos.RequestDtos;
using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.RegularUserCommand.UpdateRegularUserByUserId;

public record UpdateRegularUserByUserIdCommand(
        Guid UserId,
        string TimeZoneId,
        AddressRequestDto Address,
        PersonalInfoRequestDto PersonalInfo
    ) : IRequest<Result>;

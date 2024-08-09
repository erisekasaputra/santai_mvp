using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.UpdateRegularUserByUserId;

public record UpdateRegularUserByUserIdCommand(
        Guid UserId, 
        string TimeZoneId,
        AddressRequestDto Address,
        PersonalInfoRequestDto PersonalInfo 
    ) : IRequest<Result>;

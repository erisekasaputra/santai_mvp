using Account.API.Applications.Dtos.RequestDtos;
using Core.Results; 
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateMechanicUserByUserId;

public record UpdateMechanicUserByUserIdCommand(
    Guid UserId,
    PersonalInfoRequestDto personalInfo,
    AddressRequestDto Address, 
    string TimeZoneId) : IRequest<Result>;
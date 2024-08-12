using Account.API.Applications.Dtos.RequestDtos;
using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateMechanicUserByUserId;

public record UpdateMechanicUserByUserIdCommand(
    Guid UserId, 
    AddressRequestDto Address, 
    string TimeZoneId) : IRequest<Result>;
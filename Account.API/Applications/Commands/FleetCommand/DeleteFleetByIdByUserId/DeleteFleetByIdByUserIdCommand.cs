using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Commands.FleetCommand.DeleteFleetByIdByUserId;

public record DeleteFleetByIdByUserIdCommand(Guid UserId, Guid FleetId) : IRequest<Result>; 
using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.AssignOrderToAMechanicByUserId;

public record AssignOrderToAMechanicByUserIdCommand(
    Guid OrderId,
    double Latitude,
    double Longitude) : IRequest<Result>;

using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.SetMechanicLocationByUserIdCommand;

public record SetMechanicLocationByUserIdCommand(
    Guid MechanicId, 
    double Latitude,
    double Longitude) : IRequest<Result>;

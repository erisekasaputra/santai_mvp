
using Core.Results;
using MediatR;

namespace Account.API.Applications.Commands.MechanicUserCommand.UpdateLocationByUserId;

public record UpdateLocationByUserIdCommand(Guid MechanicId, double Latitude, double Longitude) : IRequest<Result>;

using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetServiceFailedByOrderIdAndMechanicId;

public record SetServiceFailedByOrderIdAndMechanicIdCommand(Guid OrderId, Guid MechanicId, Guid FleetId, string Secret) : IRequest<Result>;

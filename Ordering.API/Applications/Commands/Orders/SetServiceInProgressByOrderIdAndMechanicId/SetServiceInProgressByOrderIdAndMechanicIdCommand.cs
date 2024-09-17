using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetServiceInProgressByOrderIdAndMechanicId;

public record SetServiceInProgressByOrderIdAndMechanicIdCommand(
    Guid OrderId, 
    Guid MechanicId, 
    string Secret,
    Guid FleetId) : IRequest<Result>;

using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.CancelOrderByMechanic;

public record CancelOrderByMechanicCommand(Guid OrderId, Guid MechanicId) : IRequest<Result>;
using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetMechanicDispatchByOrderIdAndMechanicId;

public record SetMechanicDispatchByOrderIdAndMechanicIdCommand(Guid OrderId, Guid MechanicId) : IRequest<Result>;

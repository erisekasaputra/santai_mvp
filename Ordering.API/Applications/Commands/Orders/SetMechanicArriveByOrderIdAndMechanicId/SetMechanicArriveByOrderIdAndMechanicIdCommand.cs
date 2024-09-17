using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetMechanicArriveByOrderIdAndMechanicId;

public record SetMechanicArriveByOrderIdAndMechanicIdCommand(
    Guid OrderId, 
    Guid MechanicId) : IRequest<Result>;

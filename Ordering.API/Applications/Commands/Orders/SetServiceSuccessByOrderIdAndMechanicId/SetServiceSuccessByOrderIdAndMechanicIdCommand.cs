using Core.Results;
using MediatR;

namespace Ordering.API.Applications.Commands.Orders.SetServiceSuccessByOrderIdAndMechanicId;

public record SetServiceSuccessByOrderIdAndMechanicIdCommand(
    Guid OrderId, 
    Guid MechanicId, 
    string Secret,
    Guid FleetId) : IRequest<Result>; 
